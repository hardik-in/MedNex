using MedNex_Backend.API.DTOs.Admin;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Interfaces;

namespace MedNex_Backend.API.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<AdminDto> GetAdminByIdAsync(int adminId)
        {
            var admin = await _adminRepository.GetAdminWithUserAsync(adminId);
            if (admin == null)
                throw new KeyNotFoundException("Admin not found.");
            return MapToDto(admin);
        }

        public async Task<AdminDto> GetAdminByPublicIdAsync(Guid publicId)
        {
            var admin = await _adminRepository.GetByPublicIdAsync(publicId);
            if (admin == null)
                throw new KeyNotFoundException("Admin not found.");

            var adminWithUser = await _adminRepository.GetAdminWithUserAsync(admin.Id);
            return MapToDto(adminWithUser!);
        }
        public async Task<AdminDto> GetAdminByUserIdAsync(int userId)
        {
            var admin = await _adminRepository.GetByUserIdAsync(userId);
            if (admin == null)
                throw new KeyNotFoundException("Admin not found.");
            return MapToDto(admin);
        }

        public async Task<AdminDto> GetAdminWithManagedDoctorsAsync(int adminId)
        {
            var admin = await _adminRepository.GetAdminWithManagedDoctorsAsync(adminId);
            if (admin == null)
                throw new KeyNotFoundException("Admin not found.");
            return MapToDto(admin);
        }

        public async Task<IEnumerable<AdminDto>> GetAllAdminsAsync()
        {
            var admins = await _adminRepository.GetAllWithUsersAsync();
            return admins.Select(MapToDto);
        }

        private AdminDto MapToDto(Admin admin)
        {
            return new AdminDto
            {
                PublicId = admin.PublicId,
                ReferenceId = admin.ReferenceId,
                UserId = admin.UserId,
                FirstName = admin.User.FirstName,
                LastName = admin.User.LastName,
                Email = admin.User.Email,
                PhoneNumber = admin.User.PhoneNumber,
                Department = admin.Department,
                EmployeeId = admin.EmployeeId,
                IsActive = admin.User.IsActive,
                CreatedAt = admin.CreatedAt
            };
        }
    }
}