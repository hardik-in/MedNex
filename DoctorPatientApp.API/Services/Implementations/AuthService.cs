using DoctorPatientApp.API.DTOs.Auth;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Repositories.Interfaces;
using DoctorPatientApp.API.Services.Interfaces;

namespace DoctorPatientApp.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }


        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Verify password
            var isPasswordValid = _passwordHasher.Verify(loginRequest.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Check if user is active
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated");

            // Update last login
            // Capture BEFORE overwriting
            var lastLoginAt = user.LastLoginAt;

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Generate JWT token
            var token = _tokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440), // 24 hours
                LastLoginAt = lastLoginAt
            };
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            var emailExists = await _userRepository.EmailExistsAsync(registerRequest.Email);
            if (emailExists)
                throw new InvalidOperationException("Email already registered");

            using var transaction = await _userRepository.BeginTransactionAsync();

            try
            {
                var passwordHash = _passwordHasher.Hash(registerRequest.Password);

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
                    IsActive = true
                };

                var createdUser = await _userRepository.AddAsync(user);

                switch (registerRequest.Role)
                {
                    case UserRole.Admin:
                        var admin = new Admin
                        {
                            UserId = createdUser.Id,

                            Department = string.IsNullOrWhiteSpace(registerRequest.Department)
                                ? "Administration"
                                : registerRequest.Department,

                            EmployeeId = string.IsNullOrWhiteSpace(registerRequest.EmployeeId)
                                ? $"ADM-{createdUser.Id}"
                                : registerRequest.EmployeeId
                        };

                        await _adminRepository.AddAsync(admin);
                        break;

                    case UserRole.Patient:
                        var patient = new Patient
                        {
                            UserId = createdUser.Id
                        };

                        await _patientRepository.AddAsync(patient);
                        break;

                    case UserRole.Doctor:
                        // 🚫 Doctors must be created via DoctorService.CreateDoctorAsync
                        throw new InvalidOperationException(
                            "Doctor accounts must be created via the Doctor creation endpoint.");
                }

                await transaction.CommitAsync();

                return new RegisterResponseDto
                {
                    Success = true,
                    Message = "Registration successful",
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

    }
}