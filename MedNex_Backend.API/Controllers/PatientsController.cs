using MedNex_Backend.API.DTOs.Common;
using MedNex_Backend.API.DTOs.Patient;
using MedNex_Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNex_Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        // GET api/patients
        // GET api/patients?page=1&pageSize=10&search=PAT-2025
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetAllPatients([FromQuery] PagedRequest request)
        {
            try
            {
                var result = await _patientService.GetAllPatientsPaginatedAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated patients");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }


        // GET api/patients/my
        // Must be defined BEFORE {publicId} route to avoid ASP.NET routing conflict.
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var patient = await _patientService.GetPatientByUserIdAsync(userId);
                return Ok(patient);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient profile");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/patients/{publicId}
        [HttpGet("{publicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetPatientById(Guid publicId)
        {
            try
            {
                var patient = await _patientService.GetPatientByPublicIdAsync(publicId);
                return Ok(patient);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST api/patients
        // FIX: Added [Authorize(Roles = "Admin")] — was completely public.
        // Patient self-registration goes through POST /api/auth/register (role = Patient).
        // This endpoint is for Admin creating a patient directly.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto createPatientDto)
        {
            try
            {
                var patient = await _patientService.CreatePatientAsync(createPatientDto);
                return CreatedAtAction(nameof(GetPatientById),
                    new { publicId = patient.PublicId }, patient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // PUT api/patients/{publicId}
        // FIX: Added ownership check — a Patient can only update their OWN profile.
        // Previously any authenticated Patient could update any other patient's record.
        [HttpPut("{publicId}")]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> UpdatePatient(Guid publicId, [FromBody] UpdatePatientDto updatePatientDto)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (role == "Patient")
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                    var myProfile = await _patientService.GetPatientByUserIdAsync(userId);

                    // Patients can only update their own profile
                    if (myProfile.PublicId != publicId)
                        return Forbid();
                }

                var patient = await _patientService.GetPatientByPublicIdAsync(publicId);
                var updated = await _patientService.UpdatePatientAsync(publicId, updatePatientDto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // DELETE api/patients/{publicId}
        [HttpDelete("{publicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePatient(Guid publicId)
        {
            try
            {
                var patient = await _patientService.GetPatientByPublicIdAsync(publicId);
                await _patientService.DeletePatientAsync(publicId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}