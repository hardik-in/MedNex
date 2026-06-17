using MedNex_Backend.API.Models.Entities;

namespace MedNex_Backend.API.Repositories.Interfaces
{
    public interface IAdminRepository : IGenericRepository<Admin>
    {
        Task<Admin?> GetByUserIdAsync(int userId);
        Task<Admin?> GetAdminWithUserAsync(int adminId);
        Task<Admin?> GetAdminWithManagedDoctorsAsync(int adminId);
        Task<IEnumerable<Admin>> GetAllWithUsersAsync();
    }
}