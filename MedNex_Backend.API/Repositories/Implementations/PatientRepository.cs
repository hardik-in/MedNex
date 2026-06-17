using MedNex_Backend.API.Data;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedNex_Backend.API.Repositories.Implementations
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient?> GetPatientWithUserAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.Id == patientId)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient?> GetPatientWithHistoryAsync(int patientId)
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
                .Where(p => p.Id == patientId)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient?> GetPatientWithUserAsync(Guid publicId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.PublicId == publicId)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient?> GetPatientWithHistoryAsync(Guid publicId)
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
                .Where(p => p.PublicId == publicId)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Patient>> GetAllWithUsersAsync()
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.User.LastName)
                .ToListAsync();
        }
    }
}