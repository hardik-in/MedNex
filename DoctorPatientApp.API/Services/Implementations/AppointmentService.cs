using DoctorPatientApp.API.DTOs.Appointment;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Repositories.Interfaces;
using DoctorPatientApp.API.Services.Interfaces;

namespace DoctorPatientApp.API.Services.Implementations
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

        public async Task<AppointmentDto> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsAsync(appointmentId);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            return MapToDto(appointment);
        }

        public async Task<IEnumerable<AppointmentListDto>> GetAppointmentsByPatientAsync(int patientId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientAsync(patientId);
            return appointments.Select(MapToListDto);
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
            var slots = await _timeSlotRepository.GetTimeSlotsByDoctorAndDateAsync(doctorId, date);

            return slots.Where(s => s.Status == SlotStatus.Available)
                       .Select(s => new AvailableSlotDto
                       {
                           TimeSlotId = s.Id,
                           Date = s.Date,
                           StartTime = s.StartTime,
                           EndTime = s.EndTime,
                           DurationMinutes = s.DurationMinutes,
                           IsAvailable = s.Status == SlotStatus.Available
                       });
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto)
        {
            // Validate patient exists
            var patient = await _patientRepository.GetByIdAsync(createAppointmentDto.PatientId);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found");

            // Validate doctor exists
            var doctor = await _doctorRepository.GetByIdAsync(createAppointmentDto.DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found");

            // Validate time slot exists and is available
            var timeSlot = await _timeSlotRepository.GetByIdAsync(createAppointmentDto.TimeSlotId);
            if (timeSlot == null)
                throw new KeyNotFoundException("Time slot not found");

            if (timeSlot.Status != SlotStatus.Available)
                throw new InvalidOperationException("Time slot is not available");

            // Check for conflicts
            var hasConflict = await _appointmentRepository.HasConflictingAppointmentAsync(
                createAppointmentDto.DoctorId,
                createAppointmentDto.TimeSlotId);

            if (hasConflict)
                throw new InvalidOperationException("This time slot is already booked");

            // Create appointment
            var appointment = new Appointment
            {
                PatientId = createAppointmentDto.PatientId,
                DoctorId = createAppointmentDto.DoctorId,
                TimeSlotId = createAppointmentDto.TimeSlotId,
                AppointmentDate = createAppointmentDto.AppointmentDate,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
                Status = AppointmentStatus.Pending,
                Reason = createAppointmentDto.Reason,
                Notes = createAppointmentDto.Notes
            };

            var createdAppointment = await _appointmentRepository.AddAsync(appointment);

            // Update time slot status
            timeSlot.Status = SlotStatus.Booked;
            await _timeSlotRepository.UpdateAsync(timeSlot);

            // Get with details
            var appointmentWithDetails = await _appointmentRepository.GetAppointmentWithDetailsAsync(createdAppointment.Id);

            return MapToDto(appointmentWithDetails);
        }

        public async Task<AppointmentDto> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus status)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsAsync(appointmentId);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            appointment.Status = status;

            if (status == AppointmentStatus.Completed)
                appointment.CompletedAt = DateTime.UtcNow;

            if (status == AppointmentStatus.Cancelled)
                appointment.CancelledAt = DateTime.UtcNow;

            await _appointmentRepository.UpdateAsync(appointment);

            // If cancelled, free up the time slot
            if (status == AppointmentStatus.Cancelled)
            {
                var timeSlot = await _timeSlotRepository.GetByIdAsync(appointment.TimeSlotId);
                if (timeSlot != null)
                {
                    timeSlot.Status = SlotStatus.Available;
                    await _timeSlotRepository.UpdateAsync(timeSlot);
                }
            }

            return MapToDto(appointment);
        }

        public async Task CancelAppointmentAsync(int appointmentId, string cancellationReason)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsAsync(appointmentId);

            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            if (appointment.Status == AppointmentStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed appointment");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelledAt = DateTime.UtcNow;
            appointment.CancellationReason = cancellationReason;

            await _appointmentRepository.UpdateAsync(appointment);

            // Free up the time slot
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
                throw new KeyNotFoundException("Appointment not found");

            await _appointmentRepository.SoftDeleteAsync(appointment);
        }
        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            // Fetch from repository and map to DTOs
            var appointments = await _appointmentRepository.GetAllWithDetailsAsync();
            return appointments.Select(MapToDto);
        }

        // Helper mapping methods
        private AppointmentDto MapToDto(Appointment appointment)
        {
            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}",
                DoctorId = appointment.DoctorId,
                DoctorName = $"Dr. {appointment.Doctor.User.FirstName} {appointment.Doctor.User.LastName}",
                DoctorSpecialization = appointment.Doctor.Specialization,
                TimeSlotId = appointment.TimeSlotId,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status,
                Reason = appointment.Reason,
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt,
                CompletedAt = appointment.CompletedAt,
                CancelledAt = appointment.CancelledAt,
                CancellationReason = appointment.CancellationReason
            };
        }

        private AppointmentListDto MapToListDto(Appointment appointment)
        {
            return new AppointmentListDto
            {
                Id = appointment.Id,
                PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}",
                DoctorName = $"Dr. {appointment.Doctor.User.FirstName} {appointment.Doctor.User.LastName}",
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                Status = appointment.Status,
                Reason = appointment.Reason
            };
        }
    }
}