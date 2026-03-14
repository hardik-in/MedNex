using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace DoctorPatientApp.API.Repositories.Implementations
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Patient> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient> GetPatientWithUserAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.Id == patientId && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient> GetPatientWithHistoryAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User)
                .Include(p => p.MedicalRecords)
                    .ThenInclude(mr => mr.Doctor)
                        .ThenInclude(d => d.User)
                .Include(p => p.Prescriptions)
                .Where(p => p.Id == patientId && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }
    }
}