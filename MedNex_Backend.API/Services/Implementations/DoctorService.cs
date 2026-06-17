using MedNex_Backend.API.DTOs.Common;
using MedNex_Backend.API.DTOs.Doctor;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Implementations;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Interfaces;
using MedNex_Backend.API.Utilities;

namespace MedNex_Backend.API.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAdminRepository _adminRepository;

        public DoctorService(
            IDoctorRepository doctorRepository,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IAppointmentRepository appointmentRepository,
            IAdminRepository adminRepository)
        {
            _doctorRepository = doctorRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _appointmentRepository = appointmentRepository;
            _adminRepository = adminRepository;
        }

        public async Task<DoctorDto> GetDoctorByIdAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorWithUserAsync(doctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");
            return MapToDto(doctor);
        }

        public async Task<DoctorDto> GetDoctorByUserIdAsync(int userId)
        {
            // FIX: Now returns DoctorDto instead of raw Doctor entity.
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found for this user.");
            return MapToDto(doctor);
        }

        public async Task<DoctorDto> GetDoctorByPublicIdAsync(Guid publicId)
        {
            var doctor = await _doctorRepository.GetByPublicIdAsync(publicId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            var doctorWithUser = await _doctorRepository.GetDoctorWithUserAsync(doctor.Id);
            return MapToDto(doctorWithUser!);
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
                throw new InvalidOperationException("Email already registered.");

            var year = DateTime.UtcNow.Year;

            var userCount = await _userRepository.GetYearlyCountAsync(year);

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

            var doctorCount = await _doctorRepository.GetYearlyCountAsync(year);

            var doctor = new Doctor
            {
                UserId = createdUser.Id,
                AssignedAdminId = adminId,
                Specialization = createDoctorDto.Specialization,
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

        public async Task<DoctorDto> UpdateDoctorAsync(Guid publicId, UpdateDoctorDto dto)
        {
            var doctor = await _doctorRepository.GetDoctorWithUserByPublicIdAsync(publicId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            if (dto.PhoneNumber != null)
                doctor.User.PhoneNumber = dto.PhoneNumber;
            if (dto.Specialization != null)
                doctor.Specialization = dto.Specialization;
            if (dto.CareerStartDate.HasValue)
                doctor.CareerStartDate = dto.CareerStartDate.Value;
            if (dto.Qualifications != null)
                doctor.Qualifications = dto.Qualifications;
            if (dto.Bio != null)
                doctor.Bio = dto.Bio;
            if (dto.ConsultationFee.HasValue)
                doctor.ConsultationFee = dto.ConsultationFee.Value;
            if (dto.Address != null)
                doctor.User.Address = dto.Address;
            if (dto.Email != null)
                doctor.User.Email = dto.Email;

            if (dto.AssignedAdminPublicId.HasValue)
            {
                var admin = await _adminRepository.GetByPublicIdAsync(dto.AssignedAdminPublicId.Value);
                if (admin != null)
                {
                    doctor.AssignedAdminId = admin.Id;
                }
            }

            await _userRepository.UpdateAsync(doctor.User);
            await _doctorRepository.UpdateAsync(doctor);
            return MapToDto(doctor);
        }

        public async Task DeleteDoctorAsync(Guid publicId)
        {
            var doctor = await _doctorRepository.GetDoctorWithUserByPublicIdAsync(publicId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found");

            await _doctorRepository.SoftDeleteAsync(doctor);

            if (doctor.User != null)
                await _userRepository.SoftDeleteAsync(doctor.User);
        }

        public async Task<IEnumerable<DoctorPatientDto>> GetDoctorPatientsAsync(int doctorId)
        {
            var appointments = await _appointmentRepository.GetDistinctPatientAppointmentsByDoctorAsync(doctorId);
            return appointments.Select(a => new DoctorPatientDto
            {
                PatientId = a.PatientId,
                PatientPublicId = a.Patient.PublicId,
                PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
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
                PublicId = doctor.PublicId,
                ReferenceId = doctor.ReferenceId,
                UserId = doctor.UserId,
                FirstName = doctor.User.FirstName,
                LastName = doctor.User.LastName,
                Email = doctor.User.Email,
                PhoneNumber = doctor.User.PhoneNumber,
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                YearsOfExperience = doctor.YearsOfExperience,
                CareerStartDate = doctor.CareerStartDate,
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
                PublicId = doctor.PublicId,
                ReferenceId = doctor.ReferenceId,
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
        public async Task<PagedResult<DoctorListDto>> GetAllDoctorsPaginatedAsync(PagedRequest request)
        {
            var (items, totalCount) = await _doctorRepository.GetPagedAsync(request);

            var dtos = new List<DoctorListDto>();
            foreach (var doctor in items)
            {
                var withUser = await _doctorRepository.GetDoctorWithUserAsync(doctor.Id);
                if (withUser != null)
                    dtos.Add(MapToListDto(withUser));
            }

            return PagedResult<DoctorListDto>.Create(dtos, totalCount, request);
        }
    }
}