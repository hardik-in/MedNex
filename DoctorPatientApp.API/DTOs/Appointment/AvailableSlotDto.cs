namespace DoctorPatientApp.API.DTOs.Appointment
{
    public class AvailableSlotDto
    {
        public int TimeSlotId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsAvailable { get; set; }
    }
}