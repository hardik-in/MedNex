using DoctorPatientApp.API.DTOs.TimeSlot;
using DoctorPatientApp.API.Models.Entities;

public interface ITimeSlotService
{
    Task<IEnumerable<TimeSlotDto>> CreateTimeSlotsAsync(CreateTimeSlotDto dto);
    Task<IEnumerable<TimeSlotDto>> GetSlotsByDoctorAsync(int doctorId);

    Task<IEnumerable<TimeSlotDto>> GetSlotsByDoctorAndDateAsync(int doctorId, DateTime date);

    Task DeleteSlotAsync(int id);
}
