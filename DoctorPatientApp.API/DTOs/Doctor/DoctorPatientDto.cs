using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.DTOs.Doctor
{
    public class DoctorPatientDto
    {
        public int PatientId { get; set; }

        public string PatientName { get; set; }

        public DateTime LastAppointmentDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}