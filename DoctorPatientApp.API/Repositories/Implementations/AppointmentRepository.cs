using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Repositories.Implementations
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Appointment> GetAppointmentWithDetailsAsync(int appointmentId)
        {
            return await _dbSet
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.TimeSlot)
                .Include(a => a.MedicalRecord)
                .Include(a => a.Prescriptions)
                .Where(a => a.Id == appointmentId && !a.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
        {
            return await _dbSet
            .Include(a => a.Patient)
            .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId && !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                            .Include(a => a.Doctor)                
            .ThenInclude(d => d.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.DoctorId == doctorId && !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _dbSet
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.DoctorId == doctorId
                         && a.AppointmentDate.Date == date.Date
                         && !a.IsDeleted)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            return await _dbSet
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.Status == status && !a.IsDeleted)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientId == patientId
                         && a.AppointmentDate >= today
                         && a.Status != AppointmentStatus.Cancelled
                         && a.Status != AppointmentStatus.Completed
                         && !a.IsDeleted)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsForDoctorAsync(int doctorId)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.DoctorId == doctorId
                         && a.AppointmentDate.Date == today
                         && !a.IsDeleted)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }
        public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User) // Get Doctor's name
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User) // Get Patient's name
                .Include(a => a.TimeSlot)     // Get the actual time
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasConflictingAppointmentAsync(int doctorId, int timeSlotId)
        {
            return await _dbSet
                .AnyAsync(a => a.DoctorId == doctorId
                            && a.TimeSlotId == timeSlotId
                            && a.Status != AppointmentStatus.Cancelled
                            && !a.IsDeleted);
        }
    }
}