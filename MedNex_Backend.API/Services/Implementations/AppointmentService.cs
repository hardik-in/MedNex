using MedNex_Backend.API.DTOs.Appointment;
using MedNex_Backend.API.DTOs.Common;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Interfaces;
using MedNex_Backend.API.Utilities;

namespace MedNex_Backend.API.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            ITimeSlotRepository timeSlotRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository)
        {
            _appointmentRepository = appointmentRepository;
            _timeSlotRepository = timeSlotRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        private static readonly Dictionary<AppointmentStatus, List<AppointmentStatus>> InvalidTransitions =
            new()
            {
                {
                    AppointmentStatus.Cancelled,
                    new List<AppointmentStatus>
                    {
                        AppointmentStatus.Confirmed,
                        AppointmentStatus.Completed,
                        AppointmentStatus.Pending
                    }
                },
                {
                    AppointmentStatus.Completed,
                    new List<AppointmentStatus>
                    {
                        AppointmentStatus.Confirmed,
                        AppointmentStatus.Cancelled,
                        AppointmentStatus.Pending
                    }
                },
                {
                    AppointmentStatus.NoShow,
                    new List<AppointmentStatus>
                    {
                        AppointmentStatus.Confirmed,
                        AppointmentStatus.Completed
                    }
                }
            };

        public async Task<AppointmentDto> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");
            return MapToDto(appointment);
        }

        public async Task<IEnumerable<AppointmentListDto>> GetAppointmentsByPatientAsync(int patientId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientAsync(patientId);
            return appointments.Select(MapToListDto);
        }
        public async Task<AppointmentDto> GetAppointmentByPublicIdAsync(Guid publicId)
        {
            var appointment = await _appointmentRepository.GetByPublicIdAsync(publicId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            var withDetails = await _appointmentRepository.GetAppointmentWithDetailsAsync(appointment.Id);
            return MapToDto(withDetails!);
        }

        public async Task<IEnumerable<AppointmentListDto>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId);
            return appointments.Select(MapToListDto);
        }

        public async Task<IEnumerable<AppointmentListDto>> GetTodaysAppointmentsForDoctorAsync(int doctorId)
        {
            var appointments = await _appointmentRepository.GetTodaysAppointmentsForDoctorAsync(doctorId);
            return appointments.Select(MapToListDto);
        }

        public async Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            var slots = await _timeSlotRepository.GetAvailableSlotsByDoctorAndDateAsync(doctorId, date);
            return slots.Select(s => new AvailableSlotDto
            {
                TimeSlotId = s.Id,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                DurationMinutes = s.DurationMinutes,
                IsAvailable = true
            });
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto dto)
        {
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            var timeSlot = await _timeSlotRepository.GetByIdAsync(dto.TimeSlotId);
            if (timeSlot == null)
                throw new KeyNotFoundException("Time slot not found.");

            if (timeSlot.Status != SlotStatus.Available)
                throw new InvalidOperationException("This time slot is not available.");

            var hasConflict = await _appointmentRepository.HasConflictingAppointmentAsync(
                dto.DoctorId, dto.TimeSlotId);
            if (hasConflict)
                throw new InvalidOperationException("This time slot is already booked.");

            var year = DateTime.UtcNow.Year;

            var appointmentCount = await _appointmentRepository.GetYearlyCountAsync(year);

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                TimeSlotId = dto.TimeSlotId,
                AppointmentDate = dto.AppointmentDate,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
                Status = AppointmentStatus.Pending,
                Reason = dto.Reason,
                Notes = dto.Notes,
                ReferenceId = ReferenceIdGenerator.Generate("APT", year, appointmentCount + 1)
            };

            var createdAppointment = await _appointmentRepository.AddAsync(appointment);

            timeSlot.Status = SlotStatus.Booked;
            await _timeSlotRepository.UpdateAsync(timeSlot);

            var withDetails = await _appointmentRepository.GetAppointmentWithDetailsAsync(createdAppointment.Id);
            return MapToDto(withDetails);
        }

        public async Task<AppointmentDto> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus newStatus)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            if (InvalidTransitions.TryGetValue(appointment.Status, out var blocked)
                && blocked.Contains(newStatus))
            {
                throw new InvalidOperationException(
                    $"Cannot transition appointment from '{appointment.Status}' to '{newStatus}'.");
            }

            appointment.Status = newStatus;

            if (newStatus == AppointmentStatus.Completed)
                appointment.CompletedAt = DateTime.UtcNow;

            if (newStatus == AppointmentStatus.Cancelled)
            {
                appointment.CancelledAt = DateTime.UtcNow;
                var timeSlot = await _timeSlotRepository.GetByIdAsync(appointment.TimeSlotId);
                if (timeSlot != null)
                {
                    timeSlot.Status = SlotStatus.Available;
                    await _timeSlotRepository.UpdateAsync(timeSlot);
                }
            }

            await _appointmentRepository.UpdateAsync(appointment);
            return MapToDto(appointment);
        }

        public async Task CancelAppointmentAsync(int appointmentId, string cancellationReason)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            if (appointment.Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Appointment is already cancelled.");

            if (appointment.Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed appointment.");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelledAt = DateTime.UtcNow;
            appointment.CancellationReason = cancellationReason;
            await _appointmentRepository.UpdateAsync(appointment);

            var timeSlot = await _timeSlotRepository.GetByIdAsync(appointment.TimeSlotId);
            if (timeSlot != null)
            {
                timeSlot.Status = SlotStatus.Available;
                await _timeSlotRepository.UpdateAsync(timeSlot);
            }
        }

        public async Task DeleteAppointmentAsync(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");
            await _appointmentRepository.SoftDeleteAsync(appointment);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.GetAllWithDetailsAsync();
            return appointments.Select(MapToDto);
        }

        private AppointmentDto MapToDto(Appointment a)
        {
            return new AppointmentDto
            {
                PublicId = a.PublicId,
                ReferenceId = a.ReferenceId,
                PatientPublicId = a.Patient.PublicId,
                PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
                DoctorPublicId = a.Doctor.PublicId,
                DoctorName = $"Dr. {a.Doctor.User.FirstName} {a.Doctor.User.LastName}",
                DoctorSpecialization = a.Doctor.Specialization,
                TimeSlotPublicId = a.TimeSlot.PublicId,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Reason = a.Reason,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt,
                CompletedAt = a.CompletedAt,
                CancelledAt = a.CancelledAt,
                CancellationReason = a.CancellationReason
            };
        }

        private AppointmentListDto MapToListDto(Appointment a)
        {
            return new AppointmentListDto
            {
                PublicId = a.PublicId,
                ReferenceId = a.ReferenceId,
                PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
                DoctorName = $"Dr. {a.Doctor.User.FirstName} {a.Doctor.User.LastName}",
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                Status = a.Status,
                Reason = a.Reason
            };
        }
        public async Task<PagedResult<AppointmentListDto>> GetAllAppointmentsPaginatedAsync(PagedRequest request)
        {
            var (items, totalCount) = await _appointmentRepository.GetPagedAsync(request);
            var dtos = new List<AppointmentListDto>();
            foreach (var appt in items)
            {
                var withDetails = await _appointmentRepository.GetAppointmentWithDetailsAsync(appt.Id);
                if (withDetails != null)
                    dtos.Add(MapToListDto(withDetails));
            }

            return PagedResult<AppointmentListDto>.Create(dtos, totalCount, request);
        }

        public async Task<PagedResult<AppointmentListDto>> GetAppointmentsByPatientPaginatedAsync(
            int patientId, PagedRequest request)
        {
            var (items, totalCount) = await _appointmentRepository.GetPagedAsync(
                request,
                filter: a => a.PatientId == patientId
            );

            var dtos = new List<AppointmentListDto>();
            foreach (var appt in items)
            {
                var withDetails = await _appointmentRepository.GetAppointmentWithDetailsAsync(appt.Id);
                if (withDetails != null)
                    dtos.Add(MapToListDto(withDetails));
            }

            return PagedResult<AppointmentListDto>.Create(dtos, totalCount, request);
        }

        public async Task<PagedResult<AppointmentListDto>> GetAppointmentsByDoctorPaginatedAsync(
            int doctorId, PagedRequest request)
        {
            var (items, totalCount) = await _appointmentRepository.GetPagedAsync(
                request,
                filter: a => a.DoctorId == doctorId
            );

            var dtos = new List<AppointmentListDto>();
            foreach (var appt in items)
            {
                var withDetails = await _appointmentRepository.GetAppointmentWithDetailsAsync(appt.Id);
                if (withDetails != null)
                    dtos.Add(MapToListDto(withDetails));
            }

            return PagedResult<AppointmentListDto>.Create(dtos, totalCount, request);
        }

    }
}