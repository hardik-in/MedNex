using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.Repositories.Interfaces
{
    public interface ITimeSlotRepository : IGenericRepository<TimeSlot>
    {
        Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorAsync(int doctorId);
        Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorAndDateAsync(int doctorId, DateTime date);
        Task<IEnumerable<TimeSlot>> GetAvailableSlotsByDoctorAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<TimeSlot> GetTimeSlotWithAppointmentAsync(int timeSlotId);
        Task<bool> IsSlotAvailableAsync(int timeSlotId);
    }
}