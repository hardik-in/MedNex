using MedNex_Backend.API.DTOs.Admin;

namespace MedNex_Backend.API.Services.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardStatsDto> GetDashboardStatsAsync();
    }
}