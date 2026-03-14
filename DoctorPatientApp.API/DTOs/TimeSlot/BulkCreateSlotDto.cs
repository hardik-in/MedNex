using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.API.DTOs.TimeSlot
{
    public class BulkCreateSlotsDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public TimeSpan DayStartTime { get; set; }

        [Required]
        public TimeSpan DayEndTime { get; set; }

        public int SlotDurationMinutes { get; set; } = 30;

        public List<DayOfWeek> WorkingDays { get; set; } = new List<DayOfWeek>
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };
    }
}