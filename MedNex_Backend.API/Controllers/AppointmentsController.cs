using MedNex_Backend.API.DTOs.Appointment;
using MedNex_Backend.API.DTOs.Common;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedNex_Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly ILogger<AppointmentsController> _logger;

        // FIX: Removed junk imports — System.Diagnostics and System.Runtime.Intrinsics.Arm

        public AppointmentsController(
            IAppointmentService appointmentService,
            IPatientService patientService,
            IDoctorService doctorService,
            ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
            _logger = logger;
        }

        // GET api/appointments?page=1&pageSize=10
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAppointments([FromQuery] PagedRequest request)
        {
            try
            {
                var result = await _appointmentService.GetAllAppointmentsPaginatedAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paginated appointments");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }


        // GET api/appointments/{publicId}
        [HttpGet("{publicId}")]
        public async Task<IActionResult> GetAppointmentById(Guid publicId)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByPublicIdAsync(publicId);
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/appointments/my?page=1&pageSize=10
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyAppointments([FromQuery] PagedRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var patient = await _patientService.GetPatientByUserIdAsync(userId);
                var result = await _appointmentService.GetAppointmentsByPatientPaginatedAsync(
                    patient.InternalId, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient appointments");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/appointments/doctor/my?page=1&pageSize=10
        [HttpGet("doctor/my")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyDoctorAppointments([FromQuery] PagedRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                var result = await _appointmentService.GetAppointmentsByDoctorPaginatedAsync(
                    doctor.InternalId, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor appointments");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/appointments/doctor/{doctorPublicId}/today
        // FIX: Added [Authorize(Roles = "Admin,Doctor")] — was completely unprotected.
        [HttpGet("doctor/{doctorPublicId}/today")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetTodaysAppointmentsForDoctor(Guid doctorPublicId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByPublicIdAsync(doctorPublicId);
                var appointments = await _appointmentService.GetTodaysAppointmentsForDoctorAsync(doctor.InternalId);
                return Ok(appointments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving today's appointments for doctor {PublicId}", doctorPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/appointments/doctor/{doctorPublicId}
        [HttpGet("doctor/{doctorPublicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAppointmentsByDoctor(Guid doctorPublicId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByPublicIdAsync(doctorPublicId);
                var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctor.InternalId);
                return Ok(appointments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for doctor {PublicId}", doctorPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/appointments/patient/{patientPublicId}
        [HttpGet("patient/{patientPublicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAppointmentsByPatient(Guid patientPublicId)
        {
            try
            {
                var patient = await _patientService.GetPatientByPublicIdAsync(patientPublicId);
                var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patient.InternalId);
                return Ok(appointments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for patient {PublicId}", patientPublicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // GET api/appointments/available-slots/{doctorPublicId}
        [HttpGet("available-slots/{doctorPublicId}")]
        public async Task<IActionResult> GetAvailableSlots(Guid doctorPublicId, [FromQuery] DateTime date)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByPublicIdAsync(doctorPublicId);
                var slots = await _appointmentService.GetAvailableSlotsAsync(doctor.InternalId, date);
                return Ok(slots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available slots");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST api/appointments
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                // Patients can only book for themselves — override any patientId from frontend
                if (role == "Patient")
                {
                    var patient = await _patientService.GetPatientByUserIdAsync(userId);
                    dto.PatientId = patient.InternalId;
                }

                var appointment = await _appointmentService.CreateAppointmentAsync(dto);
                return CreatedAtAction(nameof(GetAppointmentById),
                    new { publicId = appointment.PublicId }, appointment);
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
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // PATCH api/appointments/{publicId}/status
        // FIX: Changed from POST /cancel to PATCH /{id}/status for consistency.
        [HttpPatch("{publicId}/status")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateAppointmentStatus(Guid publicId, [FromBody] AppointmentStatus status)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByPublicIdAsync(publicId);
                var updated = await _appointmentService.UpdateAppointmentStatusAsync(appointment.InternalId, status);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Status transition violations — return 422 Unprocessable Entity
                return UnprocessableEntity(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment status {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // PATCH api/appointments/{publicId}/cancel
        // FIX: Added [Authorize] — was completely unprotected, anyone could cancel.
        // FIX: Changed from POST to PATCH — cancellation is a partial update, not a new resource.
        [HttpPatch("{publicId}/cancel")]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> CancelAppointment(Guid publicId, [FromBody] string cancellationReason)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var appointment = await _appointmentService.GetAppointmentByPublicIdAsync(publicId);

                // Patients can only cancel their OWN appointments
                if (role == "Patient")
                {
                    var patient = await _patientService.GetPatientByUserIdAsync(userId);
                    if (appointment.PatientInternalId != patient.InternalId)
                        return Forbid();
                }

                await _appointmentService.CancelAppointmentAsync(appointment.InternalId, cancellationReason);
                return Ok(new { message = "Appointment cancelled successfully." });
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
                _logger.LogError(ex, "Error cancelling appointment {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // DELETE api/appointments/{publicId}
        [HttpDelete("{publicId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAppointment(Guid publicId)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByPublicIdAsync(publicId);
                await _appointmentService.DeleteAppointmentAsync(appointment.InternalId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment {PublicId}", publicId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}