using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.API.DTOs.TimeSlot
{
    public class CreateTimeSlotDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int DurationMinutes { get; set; } = 30;
    }
}