using MedNex_Backend.API.Data;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedNex_Backend.API.Repositories.Implementations
{
    public class TimeSlotRepository : GenericRepository<TimeSlot>, ITimeSlotRepository
    {
        public TimeSlotRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(ts => ts.Doctor).ThenInclude(d => d.User)
                .Where(ts => ts.DoctorId == doctorId)
                .OrderBy(ts => ts.Date)
                .ThenBy(ts => ts.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _dbSet
                .Include(ts => ts.Doctor).ThenInclude(d => d.User)
                .Where(ts => ts.DoctorId == doctorId && ts.Date.Date == date.Date)
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _dbSet
                .Include(ts => ts.Doctor).ThenInclude(d => d.User)
                .Where(ts => ts.DoctorId == doctorId
                          && ts.Date.Date == date.Date
                          && ts.Status == SlotStatus.Available)
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsByDoctorAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(ts => ts.Doctor).ThenInclude(d => d.User)
                .Where(ts => ts.DoctorId == doctorId
                          && ts.Date >= startDate.Date
                          && ts.Date <= endDate.Date
                          && ts.Status == SlotStatus.Available)
                .OrderBy(ts => ts.Date)
                .ThenBy(ts => ts.StartTime)
                .ToListAsync();
        }

        public async Task<TimeSlot?> GetTimeSlotWithAppointmentAsync(int timeSlotId)
        {
            return await _dbSet
                .Include(ts => ts.Doctor).ThenInclude(d => d.User)
                .Include(ts => ts.Appointment)
                .Where(ts => ts.Id == timeSlotId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsSlotAvailableAsync(int timeSlotId)
        {
            var slot = await _dbSet.FindAsync(timeSlotId);
            return slot != null && slot.Status == SlotStatus.Available && !slot.IsDeleted;
        }
    }
}