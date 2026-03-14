using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

namespace DoctorPatientApp.API.Repositories.Implementations
{
    public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Prescription> GetPrescriptionWithDetailsAsync(int prescriptionId)
        {
            return await _dbSet
                .Include(p => p.Patient)
                    .ThenInclude(pt => pt.User)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                .Include(p => p.MedicalRecord)
                .Where(p => p.Id == prescriptionId && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.User)
                .Include(p => p.Appointment)
                .Where(p => p.PatientId == patientId && !p.IsDeleted)
                .OrderByDescending(p => p.PrescribedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(p => p.Patient)
                    .ThenInclude(pt => pt.User)
                .Include(p => p.Appointment)
                .Where(p => p.DoctorId == doctorId && !p.IsDeleted)
                .OrderByDescending(p => p.PrescribedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetPrescriptionsByAppointmentAsync(int appointmentId)
        {
            return await _dbSet
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.User)
                .Where(p => p.AppointmentId == appointmentId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetActivePrescriptionsForPatientAsync(int patientId)
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.User)
                .Where(p => p.PatientId == patientId
                         && p.IsActive
                         && (!p.EndDate.HasValue || p.EndDate.Value >= today)
                         && !p.IsDeleted)
                .OrderBy(p => p.MedicationName)
                .ToListAsync();
        }
    }
}