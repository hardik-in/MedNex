using DoctorPatientApp.API.DTOs.Appointment;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Services.Implementations;
using DoctorPatientApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using DoctorPatientApp.API.Services.Interfaces;

namespace DoctorPatientApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IPatientService patientService,
            IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

[HttpGet("my")]
[Authorize(Roles = "Patient")]
public async Task<IActionResult> GetMyAppointments()
{
    try
    {
        // Get logged-in user's ID from JWT
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        // Get patient linked to this user
        var patient = await _patientService.GetByUserIdAsync(userId);

        // Fetch only THIS patient's appointments
        var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patient.Id);

        return Ok(appointments);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}


        [HttpGet("doctor/my")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyDoctorAppointments()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);

                var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctor.Id);

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("doctor/{doctorId}/today")]
        public async Task<IActionResult> GetTodaysAppointmentsForDoctor(int doctorId)
        {
            try
            {
                var appointments = await _appointmentService.GetTodaysAppointmentsForDoctorAsync(doctorId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("available-slots/{doctorId}")]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, [FromQuery] DateTime date)
        {
            try
            {
                var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, date);
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto createAppointmentDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var role = User.FindFirst(ClaimTypes.Role).Value;

                // If patient, force appointment to be for SELF only
                if (role == "Patient")
                {
                    var patient = await _patientService.GetByUserIdAsync(userId);

                    // Override any patientId sent by frontend
                    createAppointmentDto.PatientId = patient.Id;
                }

                // Admin can create for anyone (allowed)
                // Doctor usually should NOT create appointments (optional rule)

                var appointment = await _appointmentService.CreateAppointmentAsync(createAppointmentDto);

                return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
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
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] AppointmentStatus status)
        {
            try
            {
                var appointment = await _appointmentService.UpdateAppointmentStatusAsync(id, status);
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id, [FromBody] string cancellationReason)
        {
            try
            {
                await _appointmentService.CancelAppointmentAsync(id, cancellationReason);
                return Ok(new { message = "Appointment cancelled successfully" });
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
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            try
            {
                await _appointmentService.DeleteAppointmentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only Admins should see the master list
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                // This method must exist in your IAppointmentService
                var appointments = await _appointmentService.GetAllAppointmentsAsync();
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAppointmentsByDoctor(int doctorId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAppointmentsByPatient(int patientId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}