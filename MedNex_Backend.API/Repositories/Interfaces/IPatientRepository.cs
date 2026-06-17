using MedNex_Backend.API.Models.Entities;

namespace MedNex_Backend.API.Repositories.Interfaces
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<Patient?> GetByUserIdAsync(int userId);
        Task<IEnumerable<Patient>> GetAllWithUsersAsync();
        Task<Patient?> GetPatientWithUserAsync(int patientId);
        Task<Patient?> GetPatientWithHistoryAsync(int patientId);
        Task<Patient?> GetPatientWithUserAsync(Guid publicId);
        Task<Patient?> GetPatientWithHistoryAsync(Guid publicId);
    }
}