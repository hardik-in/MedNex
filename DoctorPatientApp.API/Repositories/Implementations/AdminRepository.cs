using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Repositories.Implementations
{
    public class AdminRepository : GenericRepository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Admin> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(a => a.User)
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Admin> GetAdminWithUserAsync(int adminId)
        {
            return await _dbSet
                .Include(a => a.User)
                .Where(a => a.Id == adminId && !a.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Admin> GetAdminWithManagedDoctorsAsync(int adminId)
        {
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.ManagedDoctors)
                    .ThenInclude(d => d.User)
                .Where(a => a.Id == adminId && !a.IsDeleted)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Appointment>> GetAllWithDetailsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .ToListAsync();
        }
        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _context.Admins
                .Include(a => a.User)
                .Where(a => a.User.IsActive)
                .ToListAsync();
        }
    }
}