using MedNex_Backend.API.DTOs.Common;
using MedNex_Backend.API.DTOs.Patient;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Interfaces;
using MedNex_Backend.API.Utilities;

namespace MedNex_Backend.API.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public PatientService(
            IPatientRepository patientRepository,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _patientRepository = patientRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<PatientDto> GetPatientByIdAsync(int patientId)
        {
            var patient = await _patientRepository.GetPatientWithUserAsync(patientId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");
            return MapToDto(patient);
        }

        public async Task<PatientDto> GetPatientByPublicIdAsync(Guid publicId)
        {
            var patient = await _patientRepository.GetByPublicIdAsync(publicId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            var patientWithUser = await _patientRepository.GetPatientWithUserAsync(patient.Id);
            return MapToDto(patientWithUser!);
        }

        public async Task<PatientDto> GetPatientByUserIdAsync(int userId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(userId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");
            return MapToDto(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllWithUsersAsync();
            return patients.Select(MapToDto);
        }

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto)
        {
            var emailExists = await _userRepository.EmailExistsAsync(createPatientDto.Email);
            if (emailExists)
                throw new InvalidOperationException("Email already registered.");

            var year = DateTime.UtcNow.Year;

            // FIX: Yearly count via repository — no direct DbContext access in service.
            var userCount = await _userRepository.GetYearlyCountAsync(year);

            var user = new User
            {
                FirstName = createPatientDto.FirstName,
                LastName = createPatientDto.LastName,
                Email = createPatientDto.Email,
                PhoneNumber = createPatientDto.PhoneNumber,
                PasswordHash = _passwordHasher.Hash(createPatientDto.Password),
                Role = UserRole.Patient,
                Gender = createPatientDto.Gender,
                DateOfBirth = createPatientDto.DateOfBirth,
                Address = createPatientDto.Address,
                IsActive = true,
                ReferenceId = ReferenceIdGenerator.Generate("USR", year, userCount + 1)
            };

            var createdUser = await _userRepository.AddAsync(user);

            var patientCount = await _patientRepository.GetYearlyCountAsync(year);

            var patient = new Patient
            {
                UserId = createdUser.Id,
                BloodGroup = createPatientDto.BloodGroup,
                Allergies = createPatientDto.Allergies ?? string.Empty,
                MedicalHistory = createPatientDto.MedicalHistory ?? string.Empty,
                EmergencyContactName = createPatientDto.EmergencyContactName ?? string.Empty,
                EmergencyContactPhone = createPatientDto.EmergencyContactPhone ?? string.Empty,
                ReferenceId = ReferenceIdGenerator.Generate("PAT", year, patientCount + 1)
            };

            var createdPatient = await _patientRepository.AddAsync(patient);
            var patientWithUser = await _patientRepository.GetPatientWithUserAsync(createdPatient.Id);
            return MapToDto(patientWithUser);
        }

        public async Task<PatientDto> UpdatePatientAsync(Guid publicId, UpdatePatientDto dto)
        {
            var patient = await _patientRepository.GetPatientWithUserAsync(publicId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            if (dto.PhoneNumber != null)
                patient.User.PhoneNumber = dto.PhoneNumber;
            if (dto.BloodGroup.HasValue)
                patient.BloodGroup = dto.BloodGroup.Value;
            if (dto.Allergies != null)
                patient.Allergies = dto.Allergies;
            if (dto.MedicalHistory != null)
                patient.MedicalHistory = dto.MedicalHistory;
            if (dto.EmergencyContactName != null)
                patient.EmergencyContactName = dto.EmergencyContactName;
            if (dto.EmergencyContactPhone != null)
                patient.EmergencyContactPhone = dto.EmergencyContactPhone;
            if (dto.Address != null)
                patient.User.Address = dto.Address;

            await _userRepository.UpdateAsync(patient.User);
            await _patientRepository.UpdateAsync(patient);
            return MapToDto(patient);
        }

        public async Task DeletePatientAsync(Guid publicId)
        {
            var patient = await _patientRepository.GetPatientWithUserAsync(publicId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found");

            await _patientRepository.SoftDeleteAsync(patient);

            if (patient.User != null)
                await _userRepository.SoftDeleteAsync(patient.User);
        }

        private PatientDto MapToDto(Patient patient)
        {
            return new PatientDto
            {
                PublicId = patient.PublicId,
                ReferenceId = patient.ReferenceId,
                UserId = patient.UserId,
                FirstName = patient.User.FirstName,
                LastName = patient.User.LastName,
                Email = patient.User.Email,
                PhoneNumber = patient.User.PhoneNumber,
                BloodGroup = patient.BloodGroup?.ToString(),
                Allergies = patient.Allergies,
                MedicalHistory = patient.MedicalHistory,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactPhone = patient.EmergencyContactPhone,
                Gender = patient.User.Gender,
                DateOfBirth = patient.User.DateOfBirth,
                Address = patient.User.Address,
                IsActive = patient.User.IsActive,
                CreatedAt = patient.CreatedAt
            };
        }
        public async Task<PagedResult<PatientDto>> GetAllPatientsPaginatedAsync(PagedRequest request)
        {
            var (items, totalCount) = await _patientRepository.GetPagedAsync(request);

            var dtos = new List<PatientDto>();
            foreach (var patient in items)
            {
                var withUser = await _patientRepository.GetPatientWithUserAsync(patient.Id);
                if (withUser != null)
                    dtos.Add(MapToDto(withUser));
            }

            return PagedResult<PatientDto>.Create(dtos, totalCount, request);
        }
    }
}