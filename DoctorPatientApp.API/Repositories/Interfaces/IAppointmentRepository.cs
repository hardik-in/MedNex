using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.Repositories.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<Appointment> GetAppointmentWithDetailsAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date);
        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId);
        Task<IEnumerable<Appointment>> GetTodaysAppointmentsForDoctorAsync(int doctorId);
        Task<bool> HasConflictingAppointmentAsync(int doctorId, int timeSlotId);
        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
    }
}