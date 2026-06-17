using MedNex_Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedNex_Backend.API.Controllers
{
    [Route("api/admins")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAdminDashboardService _dashboardService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAdminService adminService,
            IAdminDashboardService dashboardService,
            ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _dashboardService = dashboardService;
            _logger = logger;
        }

        // GET api/admins
        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                var admins = await _adminService.GetAllAdminsAsync();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all admins");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/admins/dashboard
        // Single endpoint that returns all dashboard stats in one call.
        // Uses Task.WhenAll internally — no need for forkJoin on the frontend.
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard stats");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/admins/{publicId}
        [HttpGet("{publicId}")]
        public async Task<IActionResult> GetAdminById(Guid publicId)
        {
            try
            {
                var admin = await _adminService.GetAdminByPublicIdAsync(publicId);
                return Ok(admin);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}