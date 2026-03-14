using DoctorPatientApp.API.DTOs.Appointment;
using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> GetAppointmentByIdAsync(int appointmentId);
        Task<IEnumerable<AppointmentListDto>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<AppointmentListDto>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<AppointmentListDto>> GetTodaysAppointmentsForDoctorAsync(int doctorId);
        Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date);
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto);
        Task<AppointmentDto> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus status);
        Task CancelAppointmentAsync(int appointmentId, string cancellationReason);
        Task DeleteAppointmentAsync(int appointmentId);
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
    }
}