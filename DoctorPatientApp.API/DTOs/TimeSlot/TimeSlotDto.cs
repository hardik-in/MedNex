using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.DTOs.TimeSlot
{
    public class TimeSlotDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public SlotStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}