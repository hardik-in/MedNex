using FluentAssertions;
using MedNex_Backend.API.DTOs.Auth;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Implementations;
using MedNex_Backend.API.Services.Interfaces;
using Moq;

namespace MedNex_Backend.Tests
{
    public class AuthServiceTests
    {
        // ── Mocks ─────────────────────────────────────────────────────────
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IAdminRepository> _mockAdminRepo;
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;

        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockAdminRepo = new Mock<IAdminRepository>();
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockTokenService = new Mock<ITokenService>();
            _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();

            _sut = new AuthService(
                _mockUserRepo.Object,
                _mockAdminRepo.Object,
                _mockPatientRepo.Object,
                _mockPasswordHasher.Object,
                _mockTokenService.Object,
                _mockRefreshTokenRepo.Object
            );
        }

        // ═══════════════════════════════════════════════════════════════════
        // LoginAsync Tests
        // ═══════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Login_WhenEmailNotFound_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange — email doesn't exist in DB
            _mockUserRepo
                .Setup(r => r.GetByEmailAsync("notfound@test.com"))
                .ReturnsAsync((User?)null);

            var dto = new LoginRequestDto
            {
                Email = "notfound@test.com",
                Password = "anypassword"
            };

            // Act
            var act = async () => await _sut.LoginAsync(dto);

            // Assert — generic message to prevent user enumeration
            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password.");
        }

        [Fact]
        public async Task Login_WhenPasswordWrong_ShouldThrowUnauthorizedAccessException()
        {
            var user = BuildActiveUser();

            _mockUserRepo
                .Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            // Password hasher returns false — wrong password
            _mockPasswordHasher
                .Setup(h => h.Verify("wrongpassword", user.PasswordHash))
                .Returns(false);

            var dto = new LoginRequestDto
            {
                Email = user.Email,
                Password = "wrongpassword"
            };

            var act = async () => await _sut.LoginAsync(dto);

            // Same generic message — attacker can't tell if email or password was wrong
            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password.");
        }

        [Fact]
        public async Task Login_WhenAccountDeactivated_ShouldThrowUnauthorizedAccessException()
        {
            var user = BuildActiveUser();
            user.IsActive = false; // deactivated account

            _mockUserRepo
                .Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _mockPasswordHasher
                .Setup(h => h.Verify("Test@1234", user.PasswordHash))
                .Returns(true);

            var dto = new LoginRequestDto
            {
                Email = user.Email,
                Password = "Test@1234"
            };

            var act = async () => await _sut.LoginAsync(dto);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*deactivated*");
        }

        [Fact]
        public async Task Login_WhenValidCredentials_ShouldReturnTokenAndUpdateLastLogin()
        {
            var user = BuildActiveUser();
            var previousLoginAt = new DateTime(2025, 1, 1);
            user.LastLoginAt = previousLoginAt;

            _mockUserRepo
                .Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _mockPasswordHasher
                .Setup(h => h.Verify("Test@1234", user.PasswordHash))
                .Returns(true);

            _mockUserRepo
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _mockTokenService
                .Setup(t => t.GenerateToken(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns("fake-jwt-token");

            _mockTokenService
                .Setup(t => t.GetExpirationMinutes())
                .Returns(60);

            _mockTokenService
                .Setup(t => t.GenerateRefreshToken())
                .Returns("fake-refresh-token");

            _mockRefreshTokenRepo
                .Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _mockRefreshTokenRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var dto = new LoginRequestDto
            {
                Email = user.Email,
                Password = "Test@1234"
            };

            var result = await _sut.LoginAsync(dto);

            // Token was issued
            result.Token.Should().Be("fake-jwt-token");
            result.RefreshToken.Should().Be("fake-refresh-token");

            // LastLoginAt returned is the PREVIOUS login time (for "Last seen" UI)
            result.LastLoginAt.Should().Be(previousLoginAt);

            // User's LastLoginAt was updated on the entity
            user.LastLoginAt.Should().NotBe(previousLoginAt);
            user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Login_WhenValid_ShouldCallUpdateAsyncOnce()
        {
            var user = BuildActiveUser();

            _mockUserRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _mockPasswordHasher.Setup(h => h.Verify("Test@1234", user.PasswordHash)).Returns(true);
            _mockUserRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockTokenService.Setup(t => t.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("token");
            _mockTokenService.Setup(t => t.GetExpirationMinutes()).Returns(60);
            _mockTokenService.Setup(t => t.GenerateRefreshToken()).Returns("refresh");
            _mockRefreshTokenRepo.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
            _mockRefreshTokenRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _sut.LoginAsync(new LoginRequestDto { Email = user.Email, Password = "Test@1234" });

            // Verify UpdateAsync was called exactly once — not zero, not twice
            _mockUserRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        // ═══════════════════════════════════════════════════════════════════
        // RegisterAsync Tests
        // ═══════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Register_WhenDoctorRole_ShouldThrowInvalidOperationException()
        {
            // Doctors cannot self-register — must be created by Admin
            var dto = new RegisterRequestDto
            {
                Email = "doctor@test.com",
                Role = UserRole.Doctor
            };

            var act = async () => await _sut.RegisterAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Doctor accounts*");
        }

        [Fact]
        public async Task Register_WhenAdminRoleWithWrongCode_ShouldThrowUnauthorizedException()
        {
            var dto = new RegisterRequestDto
            {
                Email = "admin@test.com",
                Role = UserRole.Admin,
                AdminRegistrationCode = "WRONG-CODE"
            };

            var act = async () => await _sut.RegisterAsync(dto);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Invalid admin registration code*");
        }

        [Fact]
        public async Task Register_WhenAdminRoleWithNoCode_ShouldThrowUnauthorizedException()
        {
            var dto = new RegisterRequestDto
            {
                Email = "admin@test.com",
                Role = UserRole.Admin,
                AdminRegistrationCode = null // no code provided
            };

            var act = async () => await _sut.RegisterAsync(dto);

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Invalid admin registration code*");
        }

        [Fact]
        public async Task Register_WhenEmailAlreadyExists_ShouldThrowInvalidOperationException()
        {
            _mockUserRepo
                .Setup(r => r.EmailExistsAsync("existing@test.com"))
                .ReturnsAsync(true);

            var dto = new RegisterRequestDto
            {
                Email = "existing@test.com",
                Role = UserRole.Patient,
                FirstName = "John",
                LastName = "Doe",
                Password = "Test@1234",
                PhoneNumber = "+919876543210"
            };

            var act = async () => await _sut.RegisterAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Email already registered*");
        }

        [Fact]
        public async Task Register_WhenValidPatient_ShouldReturnSuccessResponse()
        {
            var dto = BuildPatientRegisterDto();

            _mockUserRepo.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
            _mockUserRepo.Setup(r => r.GetYearlyCountAsync(It.IsAny<int>())).ReturnsAsync(0);
            _mockPatientRepo.Setup(r => r.GetYearlyCountAsync(It.IsAny<int>())).ReturnsAsync(0);
            _mockPasswordHasher.Setup(h => h.Hash(dto.Password)).Returns("hashed-password");

            var createdUser = new User { Id = 1, Email = dto.Email };
            _mockUserRepo
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUser);

            _mockPatientRepo
                .Setup(r => r.AddAsync(It.IsAny<Patient>()))
                .ReturnsAsync(new Patient { Id = 1 });

            // Mock transaction
            var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(default)).Returns(Task.CompletedTask);
            _mockUserRepo
                .Setup(r => r.BeginTransactionAsync())
                .ReturnsAsync(mockTransaction.Object);

            var result = await _sut.RegisterAsync(dto);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Email.Should().Be(dto.Email);
        }

        [Fact]
        public async Task Register_WhenValidPatient_ShouldHashPassword()
        {
            var dto = BuildPatientRegisterDto();

            _mockUserRepo.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
            _mockUserRepo.Setup(r => r.GetYearlyCountAsync(It.IsAny<int>())).ReturnsAsync(0);
            _mockPatientRepo.Setup(r => r.GetYearlyCountAsync(It.IsAny<int>())).ReturnsAsync(0);
            _mockPasswordHasher.Setup(h => h.Hash(dto.Password)).Returns("hashed-password");
            _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = 1, Email = dto.Email });
            _mockPatientRepo.Setup(r => r.AddAsync(It.IsAny<Patient>())).ReturnsAsync(new Patient());

            var mockTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(default)).Returns(Task.CompletedTask);
            _mockUserRepo.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);

            await _sut.RegisterAsync(dto);

            // Password was hashed — Hash() was called with the plain text password
            _mockPasswordHasher.Verify(h => h.Hash("Test@1234"), Times.Once);
        }

        // ═══════════════════════════════════════════════════════════════════
        // RefreshAsync Tests
        // ═══════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Refresh_WhenTokenNotFound_ShouldThrowUnauthorizedException()
        {
            _mockRefreshTokenRepo
                .Setup(r => r.GetByTokenAsync("invalid-token"))
                .ReturnsAsync((RefreshToken?)null);

            var act = async () => await _sut.RefreshAsync("invalid-token");

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*Invalid refresh token*");
        }

        [Fact]
        public async Task Refresh_WhenTokenRevoked_ShouldThrowUnauthorizedException()
        {
            var revokedToken = new RefreshToken
            {
                Token = "revoked-token",
                IsRevoked = true,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = BuildActiveUser()
            };

            _mockRefreshTokenRepo
                .Setup(r => r.GetByTokenAsync("revoked-token"))
                .ReturnsAsync(revokedToken);

            var act = async () => await _sut.RefreshAsync("revoked-token");

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*expired or been revoked*");
        }

        [Fact]
        public async Task Refresh_WhenTokenExpired_ShouldThrowUnauthorizedException()
        {
            var expiredToken = new RefreshToken
            {
                Token = "expired-token",
                IsRevoked = false,
                ExpiresAt = DateTime.UtcNow.AddDays(-1), // expired yesterday
                User = BuildActiveUser()
            };

            _mockRefreshTokenRepo
                .Setup(r => r.GetByTokenAsync("expired-token"))
                .ReturnsAsync(expiredToken);

            var act = async () => await _sut.RefreshAsync("expired-token");

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("*expired or been revoked*");
        }

        [Fact]
        public async Task Refresh_WhenValid_ShouldRevokeOldTokenAndIssueNewPair()
        {
            var user = BuildActiveUser();
            var activeToken = new RefreshToken
            {
                Id = 1,
                Token = "valid-refresh-token",
                IsRevoked = false,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = user
            };

            _mockRefreshTokenRepo
                .Setup(r => r.GetByTokenAsync("valid-refresh-token"))
                .ReturnsAsync(activeToken);

            _mockRefreshTokenRepo
                .Setup(r => r.UpdateAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _mockRefreshTokenRepo
                .Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _mockRefreshTokenRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockTokenService
                .Setup(t => t.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("new-access-token");

            _mockTokenService
                .Setup(t => t.GenerateRefreshToken())
                .Returns("new-refresh-token");

            _mockTokenService
                .Setup(t => t.GetExpirationMinutes())
                .Returns(60);

            var result = await _sut.RefreshAsync("valid-refresh-token");

            // New tokens issued
            result.AccessToken.Should().Be("new-access-token");
            result.RefreshToken.Should().Be("new-refresh-token");

            // Old token was revoked (token rotation)
            activeToken.IsRevoked.Should().BeTrue();
            activeToken.RevokedAt.Should().NotBeNull();

            // UpdateAsync called on the old token
            _mockRefreshTokenRepo.Verify(r => r.UpdateAsync(activeToken), Times.Once);

            // AddAsync called for new refresh token
            _mockRefreshTokenRepo.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
        }

        // ═══════════════════════════════════════════════════════════════════
        // LogoutAsync Tests
        // ═══════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Logout_WhenTokenValid_ShouldRevokeIt()
        {
            var activeToken = new RefreshToken
            {
                Token = "active-token",
                IsRevoked = false,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = BuildActiveUser()
            };

            _mockRefreshTokenRepo
                .Setup(r => r.GetByTokenAsync("active-token"))
                .ReturnsAsync(activeToken);

            _mockRefreshTokenRepo
                .Setup(r => r.UpdateAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _mockRefreshTokenRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            await _sut.LogoutAsync("active-token");

            activeToken.IsRevoked.Should().BeTrue();
            activeToken.RevokedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Logout_WhenTokenNotFound_ShouldNotThrow()
        {
            // Logout should silently succeed even if token is invalid —
            // logout must never fail from the user's perspective
            _mockRefreshTokenRepo
                .Setup(r => r.GetByTokenAsync("nonexistent-token"))
                .ReturnsAsync((RefreshToken?)null);

            var act = async () => await _sut.LogoutAsync("nonexistent-token");

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Logout_WhenTokenAlreadyRevoked_ShouldNotThrow()
        {
            var revokedToken = new RefreshToken
            {
                Token = "already-revoked",
                IsRevoked = true,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = BuildActiveUser()
            };

            _mockRefreshTokenRepo
                .Setup(r => r.GetByTokenAsync("already-revoked"))
                .ReturnsAsync(revokedToken);

            var act = async () => await _sut.LogoutAsync("already-revoked");

            // Should not throw — idempotent logout
            await act.Should().NotThrowAsync();

            // UpdateAsync should NOT be called — no point updating an already-revoked token
            _mockRefreshTokenRepo.Verify(
                r => r.UpdateAsync(It.IsAny<RefreshToken>()), Times.Never);
        }

        // ═══════════════════════════════════════════════════════════════════
        // Private helpers
        // ═══════════════════════════════════════════════════════════════════

        private static User BuildActiveUser() => new User
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Email = "test@mednex.com",
            PasswordHash = "hashed-password",
            Role = UserRole.Patient,
            IsActive = true,
            LastLoginAt = null
        };

        private static RegisterRequestDto BuildPatientRegisterDto() => new RegisterRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            Password = "Test@1234",
            PhoneNumber = "+919876543210",
            Role = UserRole.Patient,
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990, 5, 15)
        };
    }
}