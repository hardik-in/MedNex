using DoctorPatientApp.API.Models.Entities;

namespace DoctorPatientApp.API.Repositories.Interfaces
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<Patient> GetByUserIdAsync(int userId);
        Task<Patient> GetPatientWithUserAsync(int patientId);
        Task<Patient> GetPatientWithHistoryAsync(int patientId);

    }
}