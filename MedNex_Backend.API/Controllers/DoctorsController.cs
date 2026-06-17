using MedNex_Backend.API.DTOs.Common;
using MedNex_Backend.API.DTOs.Doctor;
using MedNex_Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNex_Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IAdminService _adminService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(
            IDoctorService doctorService,
            IAdminService adminService,
            ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _adminService = adminService;
            _logger = logger;
        }

        // GET api/doctors
        // Public — patients need to browse doctors without logging in
        // GET api/doctors?page=1&pageSize=10&search=cardio
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors([FromQuery] PagedRequest request)
        {
            try
            {
                var result = await _doctorService.GetAllDoctorsPaginatedAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated doctors");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/doctors/{publicId}
        [HttpGet("{publicId}")]
        public async Task<IActionResult> GetDoctorById(Guid publicId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByPublicIdAsync(publicId);
                return Ok(doctor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/doctors/specialization/{specialization}
        [HttpGet("specialization/{specialization}")]
        public async Task<IActionResult> GetDoctorsBySpecialization(string specialization)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specialization);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors by specialization {Specialization}", specialization);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/doctors/my
        // FIX: Was calling GetDoctorByUserIdAsync then GetDoctorByIdAsync(doctor.Id) —
        // two service calls to get the same data. GetDoctorByUserIdAsync now returns
        // DoctorDto directly so the second call is completely unnecessary.
        [HttpGet("my")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                return Ok(doctor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor profile for user {UserId}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/doctors/my/patients
        [HttpGet("my/patients")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyPatients()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                var patients = await _doctorService.GetDoctorPatientsAsync(doctor.InternalId);
                return Ok(patients);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients for doctor");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/doctors/admin/{adminPublicId}
        [HttpGet("admin/{adminPublicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDoctorsByAdmin(Guid adminPublicId)
        {
            try
            {
                var admin = await _adminService.GetAdminByPublicIdAsync(adminPublicId);
                var doctors = await _doctorService.GetDoctorsByAdminAsync(admin.InternalId);
                return Ok(doctors);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors for admin {AdminPublicId}", adminPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/doctors/{publicId}/patients
        [HttpGet("{publicId}/patients")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDoctorPatients(Guid publicId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByPublicIdAsync(publicId);
                var patients = await _doctorService.GetDoctorPatientsAsync(doctor.InternalId);
                return Ok(patients);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients for doctor {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST api/doctors
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto createDoctorDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Invalid token." });

                var userId = int.Parse(userIdClaim);
                var admin = await _adminService.GetAdminByUserIdAsync(userId);
                var doctor = await _doctorService.CreateDoctorAsync(createDoctorDto, admin.InternalId);

                return CreatedAtAction(nameof(GetDoctorById),
                    new { publicId = doctor.PublicId }, doctor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // PUT api/doctors/{publicId}
        // FIX: Added ownership check — a Doctor can only update their OWN profile.
        // Previously any authenticated Doctor could update any Doctor's record.
        [HttpPut("{publicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateDoctor(Guid publicId, [FromBody] UpdateDoctorDto updateDoctorDto)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (role == "Doctor")
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                    var myProfile = await _doctorService.GetDoctorByUserIdAsync(userId);

                    // Doctors can only update their own profile
                    if (myProfile.PublicId != publicId)
                        return Forbid();
                }

                var updated = await _doctorService.UpdateDoctorAsync(publicId, updateDoctorDto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // DELETE api/doctors/{publicId}
        [HttpDelete("{publicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoctor(Guid publicId)
        {
            try
            {
                await _doctorService.DeleteDoctorAsync(publicId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}