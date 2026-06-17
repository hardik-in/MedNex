using MedNex_Backend.API.Models.Entities;

namespace MedNex_Backend.API.Repositories.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<Doctor> GetByUserIdAsync(int userId);
        Task<Doctor> GetDoctorWithUserAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization);
        Task<IEnumerable<Doctor>> GetDoctorsByAdminAsync(int adminId);
        Task<IEnumerable<Doctor>> GetActiveDoctorsAsync();
        Task<Doctor> GetDoctorWithUserByPublicIdAsync(Guid publicId);
    }
}