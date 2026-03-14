using System.ComponentModel.DataAnnotations;
using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.DTOs.Appointment
{
    public class UpdateAppointmentDto
    {
        public AppointmentStatus? Status { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; }

        [MaxLength(500)]
        public string CancellationReason { get; set; }
    }
}