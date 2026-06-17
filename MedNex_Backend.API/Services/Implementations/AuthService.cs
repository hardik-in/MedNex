using MedNex_Backend.API.DTOs.Auth;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Interfaces;
using MedNex_Backend.API.Utilities;

namespace MedNex_Backend.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        // Refresh token lifetime — 7 days.
        // User stays logged in for 7 days without re-entering password.
        // Change to shorter value (e.g. 1 day) for higher-security environments.
        private const int RefreshTokenExpiryDays = 7;

        public AuthService(
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            IPatientRepository patientRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _patientRepository = patientRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);

            // Generic error — does not reveal whether email exists (prevents enumeration)
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var isPasswordValid = _passwordHasher.Verify(loginRequest.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated. Please contact support.");

            // Capture previous login time before overwriting (shown in UI as "Last seen: X")
            var previousLoginAt = user.LastLoginAt;
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Issue access token (JWT)
            var accessToken = _tokenService.GenerateToken(
                userId: user.Id,
                email: user.Email,
                role: user.Role.ToString(),
                fullName: $"{user.FirstName} {user.LastName}"
            );

            // Issue refresh token and persist to DB
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            return new LoginResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_tokenService.GetExpirationMinutes()),
                LastLoginAt = previousLoginAt
            };
        }

        public async Task<RefreshTokenResponseDto> RefreshAsync(string refreshTokenValue)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenValue);

            if (storedToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!storedToken.IsActive)
                throw new UnauthorizedAccessException("Refresh token has expired or been revoked.");

            var user = storedToken.User;

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated.");

            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            var newAccessToken = _tokenService.GenerateToken(
                userId: user.Id,
                email: user.Email,
                role: user.Role.ToString(),
                fullName: $"{user.FirstName} {user.LastName}"
            );

            var newRefreshToken = await CreateRefreshTokenAsync(user.Id);

            await _refreshTokenRepository.SaveChangesAsync();

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_tokenService.GetExpirationMinutes())
            };
        }

        public async Task LogoutAsync(string refreshTokenValue)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshTokenValue);

            if (storedToken == null || !storedToken.IsActive)
                return;

            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedToken);
            await _refreshTokenRepository.SaveChangesAsync();
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (registerRequest.Role == UserRole.Doctor)
                throw new InvalidOperationException(
                    "Doctor accounts must be created by an Admin via the doctor management endpoint.");

            if (registerRequest.Role == UserRole.Admin)
            {
                if (string.IsNullOrWhiteSpace(registerRequest.AdminRegistrationCode) ||
                    registerRequest.AdminRegistrationCode != "MEDNEX-ADMIN-2025")
                    throw new UnauthorizedAccessException("Invalid admin registration code.");
            }

            var emailExists = await _userRepository.EmailExistsAsync(registerRequest.Email);
            if (emailExists)
                throw new InvalidOperationException("Email already registered.");

            using var transaction = await _userRepository.BeginTransactionAsync();

            try
            {
                var year = DateTime.UtcNow.Year;
                var passwordHash = _passwordHasher.Hash(registerRequest.Password);

                var userCount = await _userRepository.GetYearlyCountAsync(year);

                var user = new User
                {
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName,
                    Email = registerRequest.Email,
                    PhoneNumber = registerRequest.PhoneNumber,
                    PasswordHash = passwordHash,
                    Role = registerRequest.Role,
                    Gender = registerRequest.Gender,
                    DateOfBirth = registerRequest.DateOfBirth,
                    Address = registerRequest.Address,
                    IsActive = true,
                    ReferenceId = ReferenceIdGenerator.Generate("USR", year, userCount + 1)
                };

                var createdUser = await _userRepository.AddAsync(user);

                switch (registerRequest.Role)
                {
                    case UserRole.Admin:
                        var adminCount = await _adminRepository.GetYearlyCountAsync(year);
                        var admin = new Admin
                        {
                            UserId = createdUser.Id,
                            Department = string.IsNullOrWhiteSpace(registerRequest.Department)
                                ? "Administration"
                                : registerRequest.Department,
                            EmployeeId = string.IsNullOrWhiteSpace(registerRequest.EmployeeId)
                                ? $"ADM-{createdUser.Id}"
                                : registerRequest.EmployeeId,
                            ReferenceId = ReferenceIdGenerator.Generate("ADM", year, adminCount + 1)
                        };
                        await _adminRepository.AddAsync(admin);
                        break;

                    case UserRole.Patient:
                        var patientCount = await _patientRepository.GetYearlyCountAsync(year);
                        var patient = new Patient
                        {
                            UserId = createdUser.Id,
                            ReferenceId = ReferenceIdGenerator.Generate("PAT", year, patientCount + 1)
                        };
                        await _patientRepository.AddAsync(patient);
                        break;
                }

                await transaction.CommitAsync();

                return new RegisterResponseDto
                {
                    Success = true,
                    Message = "Registration successful.",
                    UserId = createdUser.Id,
                    Email = createdUser.Email
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ── Private Helpers ───────────────────────────────────────────────
        private async Task<RefreshToken> CreateRefreshTokenAsync(int userId)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = _tokenService.GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return refreshToken;
        }
    }
}