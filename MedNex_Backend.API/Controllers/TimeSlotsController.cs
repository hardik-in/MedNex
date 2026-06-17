using MedNex_Backend.API.DTOs.TimeSlot;
using MedNex_Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNex_Backend.API.Controllers
{
    // FIX: Added proper namespace — was in global namespace.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Doctor")]
    public class TimeSlotsController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;
        private readonly IDoctorService _doctorService;
        private readonly IAdminService _adminService;
        private readonly ILogger<TimeSlotsController> _logger;

        public TimeSlotsController(
            ITimeSlotService timeSlotService,
            IDoctorService doctorService,
            IAdminService adminService,
            ILogger<TimeSlotsController> logger)
        {
            _timeSlotService = timeSlotService;
            _doctorService = doctorService;
            _adminService = adminService;
            _logger = logger;
        }

        // POST api/timeslots
        // FIX: Added try/catch — was completely unhandled, any error crashed with no response.
        [HttpPost]
        public async Task<IActionResult> CreateTimeSlot([FromBody] CreateTimeSlotDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (role == "Doctor")
                {
                    // Doctors can only create slots for themselves
                    var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                    dto.DoctorId = doctor.InternalId;
                }
                else if (role == "Admin")
                {
                    // Admins can only create slots for doctors they manage
                    var admin = await _adminService.GetAdminByUserIdAsync(userId);
                    var doctor = await _doctorService.GetDoctorByIdAsync(dto.DoctorId);

                    if (doctor.AssignedAdminInternalId != admin.InternalId)
                        return Forbid();
                }

                var result = await _timeSlotService.CreateTimeSlotsAsync(dto);
                return StatusCode(201, result);
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
                _logger.LogError(ex, "Error creating time slots");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/timeslots/my
        [HttpGet("my")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMySlots()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                var slots = await _timeSlotService.GetSlotsByDoctorAsync(doctor.InternalId);
                return Ok(slots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor's own slots");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/timeslots/doctor/{doctorPublicId}
        [HttpGet("doctor/{doctorPublicId}")]
        public async Task<IActionResult> GetSlotsByDoctor(Guid doctorPublicId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByPublicIdAsync(doctorPublicId);
                var slots = await _timeSlotService.GetSlotsByDoctorAsync(doctor.InternalId);
                return Ok(slots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving slots for doctor {PublicId}", doctorPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/timeslots/doctor/{doctorPublicId}/date/{date}
        [HttpGet("doctor/{doctorPublicId}/date/{date}")]
        public async Task<IActionResult> GetSlotsByDoctorAndDate(Guid doctorPublicId, DateTime date)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByPublicIdAsync(doctorPublicId);
                var slots = await _timeSlotService.GetSlotsByDoctorAndDateAsync(doctor.InternalId, date);
                return Ok(slots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving slots for doctor {PublicId} on {Date}", doctorPublicId, date);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // DELETE api/timeslots/{publicId}
        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteSlot(Guid publicId)
        {
            try
            {
                var slot = await _timeSlotService.GetSlotByPublicIdAsync(publicId);
                await _timeSlotService.DeleteSlotAsync(slot.InternalId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting slot {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}