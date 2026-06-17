using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;

namespace MedNex_Backend.API.Repositories.Interfaces
{
    public interface ITimeSlotRepository : IGenericRepository<TimeSlot>
    {
        Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorAsync(int doctorId);
        Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorAndDateAsync(int doctorId, DateTime date);
        Task<IEnumerable<TimeSlot>> GetAvailableSlotsByDoctorAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<TimeSlot?> GetTimeSlotWithAppointmentAsync(int timeSlotId);
        Task<bool> IsSlotAvailableAsync(int timeSlotId);
        Task<IEnumerable<TimeSlot>> GetAvailableSlotsByDoctorAndDateAsync(int doctorId, DateTime date);
    }
}