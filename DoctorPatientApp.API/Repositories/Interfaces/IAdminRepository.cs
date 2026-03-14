using DoctorPatientApp.API.Models.Entities;

namespace DoctorPatientApp.API.Repositories.Interfaces
{
    public interface IAdminRepository : IGenericRepository<Admin>
    {
        Task<Admin> GetByUserIdAsync(int userId);
        Task<IEnumerable<Appointment>> GetAllWithDetailsAsync();
        Task<Admin> GetAdminWithUserAsync(int adminId);
        Task<Admin> GetAdminWithManagedDoctorsAsync(int adminId);
        Task<IEnumerable<Admin>> GetAllAsync();
    }
}