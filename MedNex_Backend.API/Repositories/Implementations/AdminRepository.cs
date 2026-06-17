using MedNex_Backend.API.Data;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedNex_Backend.API.Repositories.Implementations
{
    public class AdminRepository : GenericRepository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Admin?> GetByUserIdAsync(int userId)
        {
            // Global query filter handles IsDeleted — no manual check needed.
            return await _dbSet
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<Admin?> GetAdminWithUserAsync(int adminId)
        {
            return await _dbSet
                .Include(a => a.User)
                .Where(a => a.Id == adminId)
                .FirstOrDefaultAsync();
        }

        public async Task<Admin?> GetAdminWithManagedDoctorsAsync(int adminId)
        {
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.ManagedDoctors)
                    .ThenInclude(d => d.User)
                .Where(a => a.Id == adminId)
                .FirstOrDefaultAsync();
        }

        // FIX: Was GetAllAsync() overriding generic with different behavior (included User).
        // Renamed to GetAllWithUsersAsync() to be explicit. The generic GetAllAsync()
        // is still available for simple cases that don't need User navigation.
        public async Task<IEnumerable<Admin>> GetAllWithUsersAsync()
        {
            return await _dbSet
                .Include(a => a.User)
                .Where(a => a.User.IsActive)
                .OrderBy(a => a.User.LastName)
                .ToListAsync();
        }
    }
}