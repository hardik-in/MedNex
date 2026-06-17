using MedNex_Backend.API.DTOs.Common;
using MedNex_Backend.API.DTOs.Patient;

namespace MedNex_Backend.API.Services.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto> GetPatientByIdAsync(int patientId);
        Task<PatientDto> GetPatientByUserIdAsync(int userId);
        Task<PatientDto> GetPatientByPublicIdAsync(Guid publicId);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PagedResult<PatientDto>> GetAllPatientsPaginatedAsync(PagedRequest request);
        Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto);
        Task<PatientDto> UpdatePatientAsync(Guid publicId, UpdatePatientDto updatePatientDto);
        Task DeletePatientAsync(Guid publicId);
    }
}