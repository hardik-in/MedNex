namespace DoctorPatientApp.API.DTOs.Admin
{
    public class AdminDashboardDto
    {
        public string AdminName { get; set; }
        public DateTime? LastLoginTime { get; set; }

        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int TotalActiveSlots { get; set; }
    }
}