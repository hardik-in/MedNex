using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.API.DTOs.Appointment
{
    public class CreateAppointmentDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int TimeSlotId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }
    }
}