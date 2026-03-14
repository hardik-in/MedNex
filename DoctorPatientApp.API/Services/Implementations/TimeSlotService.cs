using DoctorPatientApp.API.DTOs.TimeSlot;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Repositories.Interfaces;

public class TimeSlotService : ITimeSlotService
{
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IDoctorRepository _doctorRepository;

    public TimeSlotService(
        ITimeSlotRepository timeSlotRepository,
        IDoctorRepository doctorRepository)
    {
        _timeSlotRepository = timeSlotRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<IEnumerable<TimeSlotDto>> CreateTimeSlotsAsync(CreateTimeSlotDto dto)
    {
        // ✅ Validate doctor exists
        var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
        if (doctor == null)
            throw new KeyNotFoundException("Doctor not found");

        var slots = new List<TimeSlot>();

        var currentStart = dto.StartTime;

        while (currentStart < dto.EndTime)
        {
            var currentEnd = currentStart.Add(TimeSpan.FromMinutes(dto.DurationMinutes));

            if (currentEnd > dto.EndTime)
                break;

            var slot = new TimeSlot
            {
                DoctorId = dto.DoctorId,
                Date = dto.Date.Date,
                StartTime = currentStart,
                EndTime = currentEnd,
                Status = SlotStatus.Available
            };

            slots.Add(slot);

            currentStart = currentEnd;
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
