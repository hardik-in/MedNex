using MedNex_Backend.API.DTOs.TimeSlot;

namespace MedNex_Backend.API.Services.Interfaces
{
    public interface ITimeSlotService
    {
        Task<IEnumerable<TimeSlotDto>> CreateTimeSlotsAsync(CreateTimeSlotDto dto);
        Task<TimeSlotDto> GetSlotByPublicIdAsync(Guid publicId);
        Task<IEnumerable<TimeSlotDto>> GetSlotsByDoctorAsync(int doctorId);
        Task<IEnumerable<TimeSlotDto>> GetSlotsByDoctorAndDateAsync(int doctorId, DateTime date);
        Task DeleteSlotAsync(int slotId);
    }
}