using DoctorPatientApp.API.Models.Entities;

namespace DoctorPatientApp.API.Repositories.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<Doctor> GetByUserIdAsync(int userId);
        Task<Doctor> GetDoctorWithUserAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetDoctorsBySpecializationAsync(string specialization);
        Task<IEnumerable<Doctor>> GetDoctorsByAdminAsync(int adminId);
        Task<IEnumerable<Doctor>> GetActiveDoctorsAsync();
    }
}