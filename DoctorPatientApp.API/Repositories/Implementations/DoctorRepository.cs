using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Repositories.Implementations
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Doctor> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(d => d.User)
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Doctor> GetDoctorWithUserAsync(int doctorId)
        {
            return await _dbSet
                .Include(d => d.User)
                .Include(d => d.AssignedAdmin)
                    .ThenInclude(a => a.User)
                .Where(d => d.Id == doctorId && !d.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization)
        {
            return await _dbSet
                .Include(d => d.User)
                .Where(d => d.Specialization.ToLower() == specialization.ToLower()
                         && !d.IsDeleted
                         && d.User.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByAdminAsync(int adminId)
        {
            return await _dbSet
                .Include(d => d.User)
                .Where(d => d.AssignedAdminId == adminId && !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
        {
            return await _dbSet
                .Include(d => d.User)
                .Where(d => !d.IsDeleted && d.User.IsActive)
                .ToListAsync();
        }
    }
}