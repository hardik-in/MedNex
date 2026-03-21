using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.DTOs.Doctor;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Repositories.Interfaces;
using DoctorPatientApp.API.Services.Interfaces;
using DoctorPatientApp.API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ApplicationDbContext _context;

        public DoctorService(
            IDoctorRepository doctorRepository,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IAppointmentRepository appointmentRepository,
            ApplicationDbContext context)
        {
            _doctorRepository = doctorRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _appointmentRepository = appointmentRepository;
            _context = context;
        }

        public async Task<DoctorDto> GetDoctorByIdAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorWithUserAsync(doctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found");
            return MapToDto(doctor);
        }

        public async Task<IEnumerable<DoctorListDto>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetActiveDoctorsAsync();
            return doctors.Select(MapToListDto);
        }

        public async Task<IEnumerable<DoctorListDto>> GetDoctorsBySpecializationAsync(string specialization)
        {
            var doctors = await _doctorRepository.GetDoctorsBySpecializationAsync(specialization);
            return doctors.Select(MapToListDto);
        }

        public async Task<IEnumerable<DoctorListDto>> GetDoctorsByAdminAsync(int adminId)
        {
            var doctors = await _doctorRepository.GetDoctorsByAdminAsync(adminId);
            return doctors.Select(MapToListDto);
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto, int adminId)
        {
            var emailExists = await _userRepository.EmailExistsAsync(createDoctorDto.Email);
            if (emailExists)
                throw new InvalidOperationException("Email already registered");

            var year = DateTime.UtcNow.Year;

            // ── User ReferenceId ──────────────────────────────────────────
            var userCount = await _context.Users
                .IgnoreQueryFilters()
                .CountAsync(u => u.CreatedAt.Year == year);

            var user = new User
            {
                FirstName = createDoctorDto.FirstName,
                LastName = createDoctorDto.LastName,
                Email = createDoctorDto.Email,
                PhoneNumber = createDoctorDto.PhoneNumber,
                PasswordHash = _passwordHasher.Hash(createDoctorDto.Password),
                Role = UserRole.Doctor,
                Gender = createDoctorDto.Gender,
                DateOfBirth = createDoctorDto.DateOfBirth,
                Address = createDoctorDto.Address,
                IsActive = true,
                ReferenceId = ReferenceIdGenerator.Generate("USR", year, userCount + 1)
            };

            var createdUser = await _userRepository.AddAsync(user);

            // ── Doctor ReferenceId ────────────────────────────────────────
            var doctorCount = await _context.Doctors
                .IgnoreQueryFilters()
                .CountAsync(d => d.CreatedAt.Year == year);

            var doctor = new Doctor
            {
                UserId = createdUser.Id,
                AssignedAdminId = adminId,
                Specialization = createDoctorDto.Specialization,
                CareerStartDate = DateTime.UtcNow.AddYears(-createDoctorDto.YearsOfExperience),
                LicenseNumber = string.IsNullOrWhiteSpace(createDoctorDto.LicenseNumber)
                    ? $"LIC-{createdUser.Id}"
                    : createDoctorDto.LicenseNumber,
                Qualifications = string.IsNullOrWhiteSpace(createDoctorDto.Qualifications)
                    ? "MBBS"
                    : createDoctorDto.Qualifications,
                Bio = string.IsNullOrWhiteSpace(createDoctorDto.Bio)
                    ? "New doctor profile"
                    : createDoctorDto.Bio,
                ConsultationFee = createDoctorDto.ConsultationFee,
                ReferenceId = ReferenceIdGenerator.Generate("DOC", year, doctorCount + 1)
            };

            var createdDoctor = await _doctorRepository.AddAsync(doctor);
            var doctorWithUser = await _doctorRepository.GetDoctorWithUserAsync(createdDoctor.Id);
            return MapToDto(doctorWithUser);
        }

        public async Task<DoctorDto> UpdateDoctorAsync(int doctorId, UpdateDoctorDto updateDoctorDto)
        {
            var doctor = await _doctorRepository.GetDoctorWithUserAsync(doctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found");

            if (!string.IsNullOrEmpty(updateDoctorDto.PhoneNumber))
                doctor.User.PhoneNumber = updateDoctorDto.PhoneNumber;
            if (!string.IsNullOrEmpty(updateDoctorDto.Specialization))
                doctor.Specialization = updateDoctorDto.Specialization;
            if (updateDoctorDto.YearsOfExperience.HasValue)
                doctor.CareerStartDate = DateTime.UtcNow.AddYears(-updateDoctorDto.YearsOfExperience.Value);
            if (!string.IsNullOrEmpty(updateDoctorDto.Qualifications))
                doctor.Qualifications = updateDoctorDto.Qualifications;
            if (!string.IsNullOrEmpty(updateDoctorDto.Bio))
                doctor.Bio = updateDoctorDto.Bio;
            if (updateDoctorDto.ConsultationFee.HasValue)
                doctor.ConsultationFee = updateDoctorDto.ConsultationFee.Value;
            if (!string.IsNullOrEmpty(updateDoctorDto.Address))
                doctor.User.Address = updateDoctorDto.Address;
            if (!string.IsNullOrEmpty(updateDoctorDto.Email))
                doctor.User.Email = updateDoctorDto.Email;
            if (updateDoctorDto.AssignedAdminId.HasValue)
                doctor.AssignedAdminId = updateDoctorDto.AssignedAdminId;

            await _userRepository.UpdateAsync(doctor.User);
            await _doctorRepository.UpdateAsync(doctor);
            return MapToDto(doctor);
        }

        public async Task DeleteDoctorAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found");
            await _doctorRepository.SoftDeleteAsync(doctor);
        }

        public async Task<Doctor> GetDoctorByUserIdAsync(int userId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found for this user");
            return doctor;
        }

        public async Task<IEnumerable<DoctorPatientDto>> GetDoctorPatientsAsync(int doctorId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId);
            return appointments
                .GroupBy(a => a.PatientId)
                .Select(g => g.OrderByDescending(a => a.AppointmentDate).First())
                .Select(a => new DoctorPatientDto
                {
                    PatientId = a.PatientId,
                    PatientName = a.Patient.User.FirstName + " " + a.Patient.User.LastName,
                    LastAppointmentDate = a.AppointmentDate,
                    StartTime = a.TimeSlot.StartTime,
                    EndTime = a.TimeSlot.EndTime,
                    Status = a.Status
                });
        }

        private DoctorDto MapToDto(Doctor doctor)
        {
            return new DoctorDto
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                FirstName = doctor.User.FirstName,
                LastName = doctor.User.LastName,
                Email = doctor.User.Email,
                PhoneNumber = doctor.User.PhoneNumber,
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                YearsOfExperience = doctor.YearsOfExperience,
                Qualifications = doctor.Qualifications,
                Bio = doctor.Bio,
                ConsultationFee = doctor.ConsultationFee,
                AssignedAdminId = doctor.AssignedAdminId,
                AssignedAdminName = doctor.AssignedAdmin != null
                    ? $"{doctor.AssignedAdmin.User.FirstName} {doctor.AssignedAdmin.User.LastName}"
                    : null,
                IsActive = doctor.User.IsActive,
                CreatedAt = doctor.CreatedAt
            };
        }

        private DoctorListDto MapToListDto(Doctor doctor)
        {
            return new DoctorListDto
            {
                Id = doctor.Id,
                FullName = $"{doctor.User.FirstName} {doctor.User.LastName}",
                Email = doctor.User.Email,
                PhoneNumber = doctor.User.PhoneNumber,
                LicenseNumber = doctor.LicenseNumber,
                Specialization = doctor.Specialization,
                YearsOfExperience = doctor.YearsOfExperience,
                ConsultationFee = doctor.ConsultationFee,
                AssignedAdminId = doctor.AssignedAdminId,
                AssignedAdminName = doctor.AssignedAdmin != null
                    ? $"{doctor.AssignedAdmin.User.FirstName} {doctor.AssignedAdmin.User.LastName}"
                    : null,
                IsActive = doctor.User.IsActive
            };
        }
    }
}