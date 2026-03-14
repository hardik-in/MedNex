using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Repositories.Implementations
{
    public class TimeSlotRepository : GenericRepository<TimeSlot>, ITimeSlotRepository
    {
        public TimeSlotRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(ts => ts.Doctor)
                    .ThenInclude(d => d.User)
                .Where(ts => ts.DoctorId == doctorId && !ts.IsDeleted)
                .OrderBy(ts => ts.Date)
                .ThenBy(ts => ts.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetTimeSlotsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _dbSet
                .Include(ts => ts.Doctor)
                    .ThenInclude(d => d.User)
                .Where(ts => ts.DoctorId == doctorId
                          && ts.Date.Date == date.Date
                          && !ts.IsDeleted)
                .OrderBy(ts => ts.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsByDoctorAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(ts => ts.Doctor)
                    .ThenInclude(d => d.User)
                .Where(ts => ts.DoctorId == doctorId
                          && ts.Date >= startDate.Date
                          && ts.Date <= endDate.Date
                          && ts.Status == SlotStatus.Available
                          && !ts.IsDeleted)
                .OrderBy(ts => ts.Date)
                .ThenBy(ts => ts.StartTime)
                .ToListAsync();
        }

        public async Task<TimeSlot> GetTimeSlotWithAppointmentAsync(int timeSlotId)
        {
            return await _dbSet
                .Include(ts => ts.Doctor)
                    .ThenInclude(d => d.User)
                .Include(ts => ts.Appointment)
                .Where(ts => ts.Id == timeSlotId && !ts.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsSlotAvailableAsync(int timeSlotId)
        {
            var slot = await _dbSet.FindAsync(timeSlotId);
            return slot != null && slot.Status == SlotStatus.Available && !slot.IsDeleted;
        }
    }
}