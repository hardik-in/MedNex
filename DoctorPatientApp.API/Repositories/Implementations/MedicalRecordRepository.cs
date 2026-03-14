using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Repositories.Implementations
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<MedicalRecord> GetMedicalRecordWithDetailsAsync(int recordId)
        {
            return await _dbSet
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Appointment)
                .Include(mr => mr.Prescriptions)
                .Where(mr => mr.Id == recordId && !mr.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetRecordsByPatientAsync(int patientId)
        {
            return await _dbSet
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Appointment)
                .Where(mr => mr.PatientId == patientId && !mr.IsDeleted)
                .OrderByDescending(mr => mr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetRecordsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Appointment)
                .Where(mr => mr.DoctorId == doctorId && !mr.IsDeleted)
                .OrderByDescending(mr => mr.CreatedAt)
                .ToListAsync();
        }

        public async Task<MedicalRecord> GetRecordByAppointmentAsync(int appointmentId)
        {
            return await _dbSet
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Where(mr => mr.AppointmentId == appointmentId && !mr.IsDeleted)
                .FirstOrDefaultAsync();
        }
    }
}