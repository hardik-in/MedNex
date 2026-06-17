using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;

namespace MedNex_Backend.API.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByEmailWithRoleAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
    }
}