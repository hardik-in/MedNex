using MedNex_Backend.API.Data;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedNex_Backend.API.Repositories.Implementations
{
    public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Prescription?> GetPrescriptionWithDetailsAsync(int prescriptionId)
        {
            return await _dbSet
                .Include(p => p.Patient).ThenInclude(pt => pt.User)
                .Include(p => p.Doctor).ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                .Include(p => p.MedicalRecord)
                .Where(p => p.Id == prescriptionId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.Patient).ThenInclude(pt => pt.User)
                .Include(p => p.Doctor).ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PrescribedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(p => p.Patient).ThenInclude(pt => pt.User)
                .Include(p => p.Doctor).ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                .Where(p => p.DoctorId == doctorId)
                .OrderByDescending(p => p.PrescribedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByAppointmentAsync(int appointmentId)
        {
            return await _dbSet
                .Include(p => p.Patient).ThenInclude(pt => pt.User)
                .Include(p => p.Doctor).ThenInclude(d => d.User)
                .Include(p => p.MedicalRecord)
                .Where(p => p.AppointmentId == appointmentId)
                .OrderBy(p => p.MedicationName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetActivePrescriptionsForPatientAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.Doctor).ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                .Where(p => p.PatientId == patientId && p.IsActive)
                .OrderByDescending(p => p.PrescribedDate)
                .ToListAsync();
        }
    }
}