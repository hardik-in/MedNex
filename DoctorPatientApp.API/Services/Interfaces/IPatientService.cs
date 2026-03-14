using DoctorPatientApp.API.DTOs.Patient;
using DoctorPatientApp.API.Models.Entities;

namespace DoctorPatientApp.API.Services.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto> GetPatientByIdAsync(int patientId);
        Task<Patient> GetByUserIdAsync(int userId);

        Task<PatientDto> GetPatientByUserIdAsync(int userId);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto);
        Task<PatientDto> UpdatePatientAsync(int patientId, UpdatePatientDto updatePatientDto);
        Task DeletePatientAsync(int patientId);
    }
}