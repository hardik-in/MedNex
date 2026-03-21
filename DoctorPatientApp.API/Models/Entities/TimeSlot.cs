using DoctorPatientApp.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorPatientApp.API.Models.Entities
{
    public class TimeSlot : BaseEntity
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

        [Required]
        public SlotStatus Status { get; set; } = SlotStatus.Available;

        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; }
        public Appointment? Appointment { get; set; }
        public string? ReferenceId { get; set; }
    }
}