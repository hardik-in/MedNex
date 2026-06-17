using MedNex_Backend.API.DTOs.TimeSlot;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Interfaces;
using MedNex_Backend.API.Utilities;

namespace MedNex_Backend.API.Services.Implementations
{
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
            var doctor = await _doctorRepository.GetDoctorWithUserAsync(dto.DoctorId);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            var year = DateTime.UtcNow.Year;

            var existingCount = await _timeSlotRepository.GetYearlyCountAsync(year);

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
                    DurationMinutes = dto.DurationMinutes,
                    Status = SlotStatus.Available,
                    ReferenceId = ReferenceIdGenerator.Generate("SLT", year, existingCount + slotIndex + 1)
                };

                slots.Add(slot);
                currentStart = currentEnd;
                slotIndex++;
            }

            if (!slots.Any())
                throw new InvalidOperationException(
                    "No time slots could be generated for the given time range and duration.");

            await _timeSlotRepository.AddRangeAsync(slots);

            var doctorFullName = $"{doctor.User.FirstName} {doctor.User.LastName}";

            return slots.Select(s => new TimeSlotDto
            {
                PublicId = s.PublicId,
                ReferenceId = s.ReferenceId,
                DoctorId = s.DoctorId,
                DoctorName = doctorFullName,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                DurationMinutes = dto.DurationMinutes,
                Status = s.Status,
                CreatedAt = s.CreatedAt
            });
        }
        public async Task<TimeSlotDto> GetSlotByPublicIdAsync(Guid publicId)
        {
            var slot = await _timeSlotRepository.GetByPublicIdAsync(publicId);
            if (slot == null)
                throw new KeyNotFoundException("Time slot not found.");

            return new TimeSlotDto
            {
                PublicId = slot.PublicId,
                ReferenceId = slot.ReferenceId,
                InternalId = slot.Id,
                DoctorId = slot.DoctorId,
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                DurationMinutes = (int)(slot.EndTime - slot.StartTime).TotalMinutes,
                Status = slot.Status,
                CreatedAt = slot.CreatedAt
            };
        }
        public async Task<IEnumerable<TimeSlotDto>> GetSlotsByDoctorAsync(int doctorId)
        {
            var slots = await _timeSlotRepository.GetTimeSlotsByDoctorAsync(doctorId);
            return slots.Select(s => new TimeSlotDto
            {
                PublicId = s.PublicId,
                ReferenceId = s.ReferenceId,
                DoctorId = s.DoctorId,
                DoctorName = $"{s.Doctor.User.FirstName} {s.Doctor.User.LastName}",
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
                PublicId = s.PublicId,
                ReferenceId = s.ReferenceId,
                DoctorId = s.DoctorId,
                DoctorName = $"{s.Doctor.User.FirstName} {s.Doctor.User.LastName}",
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                DurationMinutes = (int)(s.EndTime - s.StartTime).TotalMinutes,
                Status = s.Status,
                CreatedAt = s.CreatedAt
            });
        }

        public async Task DeleteSlotAsync(int slotId)
        {
            var slot = await _timeSlotRepository.GetByIdAsync(slotId);
            if (slot == null)
                throw new KeyNotFoundException("Time slot not found.");
            await _timeSlotRepository.SoftDeleteAsync(slot);
        }
    }
}