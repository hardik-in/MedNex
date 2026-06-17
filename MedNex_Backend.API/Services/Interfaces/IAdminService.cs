using MedNex_Backend.API.DTOs.Admin;

namespace MedNex_Backend.API.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDto> GetAdminByIdAsync(int adminId);
        Task<AdminDto> GetAdminByPublicIdAsync(Guid publicId);
        Task<AdminDto> GetAdminByUserIdAsync(int userId);
        Task<AdminDto> GetAdminWithManagedDoctorsAsync(int adminId);
        Task<IEnumerable<AdminDto>> GetAllAdminsAsync();
    }
}