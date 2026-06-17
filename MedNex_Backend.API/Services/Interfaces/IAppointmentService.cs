using MedNex_Backend.API.DTOs.Appointment;
using MedNex_Backend.API.DTOs.Common;
using MedNex_Backend.API.Models.Enums;

namespace MedNex_Backend.API.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> GetAppointmentByIdAsync(int appointmentId);
        Task<AppointmentDto> GetAppointmentByPublicIdAsync(Guid publicId);
        Task<IEnumerable<AppointmentListDto>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<AppointmentListDto>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<AppointmentListDto>> GetTodaysAppointmentsForDoctorAsync(int doctorId);
        Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date);
        Task<PagedResult<AppointmentListDto>> GetAllAppointmentsPaginatedAsync(PagedRequest request);
        Task<PagedResult<AppointmentListDto>> GetAppointmentsByPatientPaginatedAsync(int patientId, PagedRequest request);
        Task<PagedResult<AppointmentListDto>> GetAppointmentsByDoctorPaginatedAsync(int doctorId, PagedRequest request);

        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto);
        Task<AppointmentDto> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus newStatus);
        Task CancelAppointmentAsync(int appointmentId, string cancellationReason);
        Task DeleteAppointmentAsync(int appointmentId);
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
    }
}