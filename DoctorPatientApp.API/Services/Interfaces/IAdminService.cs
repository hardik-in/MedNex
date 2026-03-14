using DoctorPatientApp.API.DTOs.Admin;

namespace DoctorPatientApp.API.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDto> GetAdminByIdAsync(int adminId);
        Task<AdminDto> GetAdminByUserIdAsync(int userId);
        Task<AdminDto> GetAdminWithManagedDoctorsAsync(int adminId);
        Task<IEnumerable<AdminDto>> GetAllAdminsAsync();
    }
}