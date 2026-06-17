using MedNex_Backend.API.DTOs.Common;
using MedNex_Backend.API.DTOs.Doctor;

namespace MedNex_Backend.API.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorDto> GetDoctorByIdAsync(int doctorId);
        Task<DoctorDto> GetDoctorByUserIdAsync(int userId);
        Task<DoctorDto> GetDoctorByPublicIdAsync(Guid publicId);
        Task<IEnumerable<DoctorListDto>> GetAllDoctorsAsync();
        Task<IEnumerable<DoctorListDto>> GetDoctorsBySpecializationAsync(string specialization);
        Task<IEnumerable<DoctorListDto>> GetDoctorsByAdminAsync(int adminId);
        Task<PagedResult<DoctorListDto>> GetAllDoctorsPaginatedAsync(PagedRequest request);
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto, int adminId);
        Task<DoctorDto> UpdateDoctorAsync(Guid publicId, UpdateDoctorDto updateDoctorDto);
        Task DeleteDoctorAsync(Guid publicId);
        Task<IEnumerable<DoctorPatientDto>> GetDoctorPatientsAsync(int doctorId);
    }
}