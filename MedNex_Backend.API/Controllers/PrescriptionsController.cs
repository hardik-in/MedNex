using MedNex_Backend.API.DTOs.Prescription;
using MedNex_Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNex_Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPatientService _patientService;
        private readonly ILogger<PrescriptionsController> _logger;

        public PrescriptionsController(
            IPrescriptionService prescriptionService,
            IPatientService patientService,
            ILogger<PrescriptionsController> logger)
        {
            _prescriptionService = prescriptionService;
            _patientService = patientService;
            _logger = logger;
        }

        // GET api/prescriptions/{publicId}
        [HttpGet("{publicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetById(Guid publicId)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescriptionByPublicIdAsync(publicId);
                return Ok(prescription);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescription {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/prescriptions/my
        // Patients can view their own active prescriptions
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyPrescriptions()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var patient = await _patientService.GetPatientByUserIdAsync(userId);
                var prescriptions = await _prescriptionService.GetActivePrescriptionsForPatientAsync(patient.InternalId);
                return Ok(prescriptions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescriptions for patient");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/prescriptions/patient/{patientPublicId}
        [HttpGet("patient/{patientPublicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByPatient(Guid patientPublicId)
        {
            try
            {
                var patient = await _patientService.GetPatientByPublicIdAsync(patientPublicId);
                var prescriptions = await _prescriptionService.GetPrescriptionsByPatientAsync(patient.InternalId);
                return Ok(prescriptions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescriptions for patient {PublicId}", patientPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/prescriptions/appointment/{appointmentPublicId}
        [HttpGet("appointment/{appointmentPublicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByAppointment(Guid appointmentPublicId)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetPrescriptionsByAppointmentPublicIdAsync(appointmentPublicId);
                return Ok(prescriptions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prescriptions for appointment {PublicId}", appointmentPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST api/prescriptions
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create([FromBody] CreatePrescriptionDto dto)
        {
            try
            {
                var prescription = await _prescriptionService.CreatePrescriptionAsync(dto);
                return CreatedAtAction(nameof(GetById),
                    new { publicId = prescription.PublicId }, prescription);
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
                _logger.LogError(ex, "Error creating prescription");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // PATCH api/prescriptions/{publicId}/deactivate
        [HttpPatch("{publicId}/deactivate")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Deactivate(Guid publicId)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescriptionByPublicIdAsync(publicId);
                var updated = await _prescriptionService.DeactivatePrescriptionAsync(prescription.InternalId);
                return Ok(updated);
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
                _logger.LogError(ex, "Error deactivating prescription {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // DELETE api/prescriptions/{publicId}
        [HttpDelete("{publicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid publicId)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescriptionByPublicIdAsync(publicId);
                await _prescriptionService.DeletePrescriptionAsync(prescription.InternalId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting prescription {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}