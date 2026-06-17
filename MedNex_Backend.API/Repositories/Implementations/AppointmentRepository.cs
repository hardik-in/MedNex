using MedNex_Backend.API.Data;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedNex_Backend.API.Repositories.Implementations
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId)
        {
            return await _dbSet
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.TimeSlot)
                .Include(a => a.MedicalRecord)
                .Include(a => a.Prescriptions)
                .Where(a => a.Id == appointmentId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
        {
            return await _dbSet
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _dbSet
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            return await _dbSet
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.Status == status)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId
                         && a.AppointmentDate >= today
                         && a.Status != AppointmentStatus.Cancelled
                         && a.Status != AppointmentStatus.Completed)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsForDoctorAsync(int doctorId)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == today)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasConflictingAppointmentAsync(int doctorId, int timeSlotId)
        {
            return await _dbSet
                .AnyAsync(a => a.DoctorId == doctorId
                            && a.TimeSlotId == timeSlotId
                            && a.Status != AppointmentStatus.Cancelled);
        }

        // NEW: Returns one appointment per distinct patient (most recent) for a doctor.
        // GroupBy pushed to DB via EF — avoids loading all appointments into memory
        // just to group them in C# as the original code did.
        public async Task<IEnumerable<Appointment>> GetDistinctPatientAppointmentsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.DoctorId == doctorId)
                .GroupBy(a => a.PatientId)
                .Select(g => g.OrderByDescending(a => a.AppointmentDate).First())
                .ToListAsync();
        }
    }
}