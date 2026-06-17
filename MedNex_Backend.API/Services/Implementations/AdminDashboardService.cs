using MedNex_Backend.API.DTOs.Admin;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Interfaces;

namespace MedNex_Backend.API.Services.Implementations
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public AdminDashboardService(
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            var totalDoctors = await _doctorRepository.CountAsync();

            var totalPatients = await _patientRepository.CountAsync();

            var todaysAppointments = await _appointmentRepository.CountAsync(
                a => a.AppointmentDate.Date == today);

            var pendingAppointments = await _appointmentRepository.CountAsync(
                a => a.Status == AppointmentStatus.Pending);

            var completedThisMonth = await _appointmentRepository.CountAsync(
                a => a.Status == AppointmentStatus.Completed
                  && a.CompletedAt.HasValue
                  && a.CompletedAt.Value.Month == currentMonth
                  && a.CompletedAt.Value.Year == currentYear);

            var newPatientsThisMonth = await _patientRepository.CountAsync(
                p => p.CreatedAt.Month == currentMonth
                  && p.CreatedAt.Year == currentYear);

            var cancelledThisMonth = await _appointmentRepository.CountAsync(
                a => a.Status == AppointmentStatus.Cancelled
                  && a.CancelledAt.HasValue
                  && a.CancelledAt.Value.Month == currentMonth
                  && a.CancelledAt.Value.Year == currentYear);

            return new AdminDashboardStatsDto
            {
                TotalDoctors = totalDoctors,
                TotalPatients = totalPatients,
                TotalAppointmentsToday = todaysAppointments,
                PendingAppointments = pendingAppointments,
                CompletedAppointmentsThisMonth = completedThisMonth,
                NewPatientsThisMonth = newPatientsThisMonth,
                CancelledAppointmentsThisMonth = cancelledThisMonth
            };
        }
    }
}