using MedNex_Backend.API.DTOs.MedicalRecord;
using MedNex_Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedNex_Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly ILogger<MedicalRecordsController> _logger;

        public MedicalRecordsController(
            IMedicalRecordService medicalRecordService,
            ILogger<MedicalRecordsController> logger)
        {
            _medicalRecordService = medicalRecordService;
            _logger = logger;
        }

        // GET api/medicalrecords/{publicId}
        // FIX: Added role restriction — was accessible to ALL authenticated users.
        // A Patient could access any other patient's medical records.
        [HttpGet("{publicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetById(Guid publicId)
        {
            try
            {
                var record = await _medicalRecordService.GetMedicalRecordByPublicIdAsync(publicId);
                return Ok(record);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medical record {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/medicalrecords/patient/{patientPublicId}
        [HttpGet("patient/{patientPublicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByPatient(Guid patientPublicId)
        {
            try
            {
                var records = await _medicalRecordService.GetRecordsByPatientPublicIdAsync(patientPublicId);
                return Ok(records);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving records for patient {PublicId}", patientPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/medicalrecords/doctor/{doctorPublicId}
        [HttpGet("doctor/{doctorPublicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByDoctor(Guid doctorPublicId)
        {
            try
            {
                var records = await _medicalRecordService.GetRecordsByDoctorPublicIdAsync(doctorPublicId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving records for doctor {PublicId}", doctorPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/medicalrecords/appointment/{appointmentPublicId}
        [HttpGet("appointment/{appointmentPublicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByAppointment(Guid appointmentPublicId)
        {
            try
            {
                var record = await _medicalRecordService.GetRecordByAppointmentPublicIdAsync(appointmentPublicId);
                return Ok(record);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving record for appointment {PublicId}", appointmentPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST api/medicalrecords
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Create([FromBody] CreateMedicalRecordDto dto)
        {
            try
            {
                var record = await _medicalRecordService.CreateMedicalRecordAsync(dto);
                return CreatedAtAction(nameof(GetById),
                    new { publicId = record.PublicId }, record);
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
                _logger.LogError(ex, "Error creating medical record");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // PUT api/medicalrecords/{publicId}
        // FIX: Changed DTO from CreateMedicalRecordDto to UpdateMedicalRecordDto.
        // Create DTO has [Required] fields that break partial updates.
        // Update DTO has all nullable fields — send only what you want to change.
        [HttpPut("{publicId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> Update(Guid publicId, [FromBody] UpdateMedicalRecordDto dto)
        {
            try
            {
                var record = await _medicalRecordService.GetMedicalRecordByPublicIdAsync(publicId);
                var updated = await _medicalRecordService.UpdateMedicalRecordAsync(record.InternalId, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medical record {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // DELETE api/medicalrecords/{publicId}
        [HttpDelete("{publicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid publicId)
        {
            try
            {
                var record = await _medicalRecordService.GetMedicalRecordByPublicIdAsync(publicId);
                await _medicalRecordService.DeleteMedicalRecordAsync(record.InternalId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical record {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}