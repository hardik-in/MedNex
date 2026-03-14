using DoctorPatientApp.API.DTOs.Common;
using DoctorPatientApp.API.DTOs.Doctor;
using DoctorPatientApp.API.Models.Entities;

namespace DoctorPatientApp.API.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorDto> GetDoctorByIdAsync(int doctorId);
        Task<IEnumerable<DoctorListDto>> GetAllDoctorsAsync();
        Task<IEnumerable<DoctorListDto>> GetDoctorsBySpecializationAsync(string specialization);
        Task<IEnumerable<DoctorListDto>> GetDoctorsByAdminAsync(int adminId);
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto, int adminId);
        Task<Doctor> GetDoctorByUserIdAsync(int userId);

        Task<DoctorDto> UpdateDoctorAsync(int doctorId, UpdateDoctorDto updateDoctorDto);
        Task DeleteDoctorAsync(int doctorId);
        Task<IEnumerable<DoctorPatientDto>> GetDoctorPatientsAsync(int doctorId);
    }
}