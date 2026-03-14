using DoctorPatientApp.API.DTOs.Admin;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Repositories.Interfaces;
using DoctorPatientApp.API.Services.Interfaces;

namespace DoctorPatientApp.API.Services.Implementations
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
                throw new KeyNotFoundException("Admin not found");

            return MapToDto(admin);
        }

        public async Task<AdminDto> GetAdminByUserIdAsync(int userId)
        {
            var admin = await _adminRepository.GetByUserIdAsync(userId);

            if (admin == null)
                throw new KeyNotFoundException("Admin not found");

            return MapToDto(admin);
        }

        public async Task<AdminDto> GetAdminWithManagedDoctorsAsync(int adminId)
        {
            var admin = await _adminRepository.GetAdminWithManagedDoctorsAsync(adminId);

            if (admin == null)
                throw new KeyNotFoundException("Admin not found");

            return MapToDto(admin);
        }

        // Helper mapping method
        private AdminDto MapToDto(Admin admin)
        {
            return new AdminDto
            {
                Id = admin.Id,
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
        public async Task<IEnumerable<AdminDto>> GetAllAdminsAsync()
        {
            var admins = await _adminRepository.GetAllAsync();
            return admins.Select(MapToDto);
        }
    }
}