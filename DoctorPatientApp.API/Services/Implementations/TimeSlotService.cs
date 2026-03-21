using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.DTOs.TimeSlot;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Repositories.Interfaces;
using DoctorPatientApp.API.Utilities;
using Microsoft.EntityFrameworkCore;

public class TimeSlotService : ITimeSlotService
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly ApplicationDbContext _context;

    public TimeSlotService(
        ITimeSlotRepository timeSlotRepository,
        IDoctorRepository doctorRepository,
        ApplicationDbContext context)
    {
        _timeSlotRepository = timeSlotRepository;
        _doctorRepository = doctorRepository;
        _context = context;
    }

    public async Task<IEnumerable<TimeSlotDto>> CreateTimeSlotsAsync(CreateTimeSlotDto dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
        if (doctor == null)
            throw new KeyNotFoundException("Doctor not found");

        var year = DateTime.UtcNow.Year;

        // Count once before the loop — increment per slot inside
        var existingCount = await _context.TimeSlots
            .IgnoreQueryFilters()
            .CountAsync(t => t.CreatedAt.Year == year);

        var slots = new List<TimeSlot>();
        var currentStart = dto.StartTime;
        var slotIndex = 0;

        while (currentStart < dto.EndTime)
        {
            var currentEnd = currentStart.Add(TimeSpan.FromMinutes(dto.DurationMinutes));
            if (currentEnd > dto.EndTime) break;

            var slot = new TimeSlot
            {
                DoctorId = dto.DoctorId,
                Date = dto.Date.Date,
                StartTime = currentStart,
                EndTime = currentEnd,
                Status = SlotStatus.Available,
                ReferenceId = ReferenceIdGenerator.Generate("SLT", year, existingCount + slotIndex + 1)
            };

            slots.Add(slot);
            currentStart = currentEnd;
            slotIndex++;
        }

        await _timeSlotRepository.AddRangeAsync(slots);

        return slots.Select(s => new TimeSlotDto
        {
            Id = s.Id,
            DoctorId = s.DoctorId,
            DoctorName = doctor.User.FirstName + " " + doctor.User.LastName,
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            DurationMinutes = dto.DurationMinutes,
            Status = s.Status,
            CreatedAt = s.CreatedAt
        });
    }

    public async Task<IEnumerable<TimeSlotDto>> GetSlotsByDoctorAsync(int doctorId)
    {
        var slots = await _timeSlotRepository.GetTimeSlotsByDoctorAsync(doctorId);
        return slots.Select(s => new TimeSlotDto
        {
            Id = s.Id,
            DoctorId = s.DoctorId,
            DoctorName = s.Doctor.User.FirstName + " " + s.Doctor.User.LastName,
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            DurationMinutes = (int)(s.EndTime - s.StartTime).TotalMinutes,
            Status = s.Status,
            CreatedAt = s.CreatedAt
        });
    }

    public async Task<IEnumerable<TimeSlotDto>> GetSlotsByDoctorAndDateAsync(int doctorId, DateTime date)
    {
        var slots = await _timeSlotRepository.GetTimeSlotsByDoctorAndDateAsync(doctorId, date);
        return slots.Select(s => new TimeSlotDto
        {
            Id = s.Id,
            DoctorId = s.DoctorId,
            DoctorName = s.Doctor.User.FirstName + " " + s.Doctor.User.LastName,
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            DurationMinutes = (int)(s.EndTime - s.StartTime).TotalMinutes,
            Status = s.Status,
            CreatedAt = s.CreatedAt
        });
    }

    public async Task DeleteSlotAsync(int id)
    {
        var slot = await _timeSlotRepository.GetByIdAsync(id);
        if (slot == null)
            throw new KeyNotFoundException("Time slot not found");
        await _timeSlotRepository.DeleteAsync(slot);
    }
}