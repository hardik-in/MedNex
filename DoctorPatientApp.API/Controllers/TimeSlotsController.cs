using DoctorPatientApp.API.DTOs.TimeSlot;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Doctor")]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService;
    private readonly IDoctorService _doctorService;
    private readonly IAdminService _adminService;

    public TimeSlotsController(
        ITimeSlotService timeSlotService,
        IDoctorService doctorService,
        IAdminService adminService)
    {
        _timeSlotService = timeSlotService;
        _doctorService = doctorService;
        _adminService = adminService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTimeSlot(CreateTimeSlotDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var role = User.FindFirst(ClaimTypes.Role).Value;

        if (role == "Doctor")
        {
            // Doctor → ONLY self
            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
            dto.DoctorId = doctor.Id;
        }
        else if (role == "Admin")
        {
            // Admin → ONLY their own doctors
            var admin = await _adminService.GetAdminByUserIdAsync(userId);

            var doctor = await _doctorService.GetDoctorByIdAsync(dto.DoctorId);

            if (doctor.AssignedAdminId != admin.Id)
                return Forbid("You do not manage this doctor.");
        }

        var result = await _timeSlotService.CreateTimeSlotsAsync(dto);

        return Ok(result);
    }
    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetSlotsByDoctor(int doctorId)
    {
        var slots = await _timeSlotService.GetSlotsByDoctorAsync(doctorId);

        return Ok(slots);
    }

    [HttpGet("doctor/{doctorId}/date/{date}")]
    public async Task<IActionResult> GetSlotsByDoctorAndDate(int doctorId, DateTime date)
    {
        var slots = await _timeSlotService.GetSlotsByDoctorAndDateAsync(doctorId, date);

        return Ok(slots);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSlot(int id)
    {
        await _timeSlotService.DeleteSlotAsync(id);
        return NoContent();
    }
}
