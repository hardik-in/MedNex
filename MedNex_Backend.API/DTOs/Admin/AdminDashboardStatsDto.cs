namespace MedNex_Backend.API.DTOs.Admin
{
    public class AdminDashboardStatsDto
    {
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointmentsToday { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedAppointmentsThisMonth { get; set; }
        public int NewPatientsThisMonth { get; set; }
        public int CancelledAppointmentsThisMonth { get; set; }
    }
}