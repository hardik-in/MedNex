using MedNex_Backend.API.Models.Enums;
using System.Text.Json.Serialization;

namespace MedNex_Backend.API.DTOs.TimeSlot
{
    public class TimeSlotDto
    {
        public Guid PublicId { get; set; }
        public string? ReferenceId { get; set; }
        [JsonIgnore]
        public int InternalId { get; set; }
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