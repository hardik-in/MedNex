using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.DTOs.Patient;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Repositories.Interfaces;
using DoctorPatientApp.API.Services.Interfaces;
using DoctorPatientApp.API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ApplicationDbContext _context;

        public PatientService(
            IPatientRepository patientRepository,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ApplicationDbContext context)
        {
            _patientRepository = patientRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _context = context;
        }

        public async Task<PatientDto> GetPatientByIdAsync(int patientId)
        {
            var patient = await _patientRepository.GetPatientWithUserAsync(patientId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found");
            return MapToDto(patient);
        }

        public async Task<PatientDto> GetPatientByUserIdAsync(int userId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(userId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found");
            return MapToDto(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var patientsWithUsers = new List<Patient>();
            foreach (var patient in patients)
            {
                var patientWithUser = await _patientRepository.GetPatientWithUserAsync(patient.Id);
                if (patientWithUser != null)
                    patientsWithUsers.Add(patientWithUser);
            }
            return patientsWithUsers.Select(MapToDto);
        }

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto)
        {
            var emailExists = await _userRepository.EmailExistsAsync(createPatientDto.Email);
            if (emailExists)
                throw new InvalidOperationException("Email already registered");

            var year = DateTime.UtcNow.Year;

            // ── User ReferenceId ──────────────────────────────────────────
            var userCount = await _context.Users
                .IgnoreQueryFilters()
                .CountAsync(u => u.CreatedAt.Year == year);

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

            // ── Patient ReferenceId ───────────────────────────────────────
            var patientCount = await _context.Patients
                .IgnoreQueryFilters()
                .CountAsync(p => p.CreatedAt.Year == year);

            var patient = new Patient
            {
                UserId = createdUser.Id,
                BloodGroup = createPatientDto.BloodGroup,
                Allergies = createPatientDto.Allergies,
                MedicalHistory = createPatientDto.MedicalHistory,
                EmergencyContactName = createPatientDto.EmergencyContactName,
                EmergencyContactPhone = createPatientDto.EmergencyContactPhone,
                ReferenceId = ReferenceIdGenerator.Generate("PAT", year, patientCount + 1)
            };

            var createdPatient = await _patientRepository.AddAsync(patient);
            var patientWithUser = await _patientRepository.GetPatientWithUserAsync(createdPatient.Id);
            return MapToDto(patientWithUser);
        }

        public async Task<PatientDto> UpdatePatientAsync(int patientId, UpdatePatientDto updatePatientDto)
        {
            var patient = await _patientRepository.GetPatientWithUserAsync(patientId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found");

            if (!string.IsNullOrEmpty(updatePatientDto.PhoneNumber))
                patient.User.PhoneNumber = updatePatientDto.PhoneNumber;
            if (updatePatientDto.BloodGroup.HasValue)
                patient.BloodGroup = updatePatientDto.BloodGroup.Value;
            if (!string.IsNullOrEmpty(updatePatientDto.Allergies))
                patient.Allergies = updatePatientDto.Allergies;
            if (!string.IsNullOrEmpty(updatePatientDto.MedicalHistory))
                patient.MedicalHistory = updatePatientDto.MedicalHistory;
            if (!string.IsNullOrEmpty(updatePatientDto.EmergencyContactName))
                patient.EmergencyContactName = updatePatientDto.EmergencyContactName;
            if (!string.IsNullOrEmpty(updatePatientDto.EmergencyContactPhone))
                patient.EmergencyContactPhone = updatePatientDto.EmergencyContactPhone;
            if (!string.IsNullOrEmpty(updatePatientDto.Address))
                patient.User.Address = updatePatientDto.Address;

            await _userRepository.UpdateAsync(patient.User);
            await _patientRepository.UpdateAsync(patient);
            return MapToDto(patient);
        }

        public async Task DeletePatientAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found");
            await _patientRepository.SoftDeleteAsync(patient);
        }

        public async Task<Patient> GetByUserIdAsync(int userId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(userId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found for this user");
            return patient;
        }

        private PatientDto MapToDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                FirstName = patient.User.FirstName,
                LastName = patient.User.LastName,
                Email = patient.User.Email,
                PhoneNumber = patient.User.PhoneNumber,
                BloodGroup = patient.BloodGroup,
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
    }
}