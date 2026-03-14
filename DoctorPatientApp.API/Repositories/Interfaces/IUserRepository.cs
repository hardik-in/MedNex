using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;

namespace DoctorPatientApp.API.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User> GetByEmailWithRoleAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
    }
}