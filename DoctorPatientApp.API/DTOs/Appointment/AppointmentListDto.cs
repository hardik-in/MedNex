using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.DTOs.Appointment
{
    public class AppointmentListDto
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Reason { get; set; }
    }
}