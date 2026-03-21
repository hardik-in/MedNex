using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.DTOs.Prescription;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Repositories.Interfaces;
using DoctorPatientApp.API.Services.Interfaces;
using DoctorPatientApp.API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Services.Implementations
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ApplicationDbContext _context;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IAppointmentRepository appointmentRepository,
            ApplicationDbContext context)
        {
            _prescriptionRepository = prescriptionRepository;
            _appointmentRepository = appointmentRepository;
            _context = context;
        }

        public async Task<PrescriptionDto> GetPrescriptionByIdAsync(int prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetPrescriptionWithDetailsAsync(prescriptionId);
            if (prescription == null)
                throw new KeyNotFoundException("Prescription not found");
            return MapToDto(prescription);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByPatientAsync(int patientId)
        {
            var prescriptions = await _prescriptionRepository.GetPrescriptionsByPatientAsync(patientId);
            return prescriptions.Select(MapToDto);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByDoctorAsync(int doctorId)
        {
            var prescriptions = await _prescriptionRepository.GetPrescriptionsByDoctorAsync(doctorId);
            return prescriptions.Select(MapToDto);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByAppointmentAsync(int appointmentId)
        {
            var prescriptions = await _prescriptionRepository.GetPrescriptionsByAppointmentAsync(appointmentId);
            return prescriptions.Select(MapToDto);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetActivePrescriptionsForPatientAsync(int patientId)
        {
            var prescriptions = await _prescriptionRepository.GetActivePrescriptionsForPatientAsync(patientId);
            return prescriptions.Select(MapToDto);
        }

        public async Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto dto)
        {
            // Validate appointment exists
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            // ── Prescription ReferenceId ──────────────────────────────────
            var year = DateTime.UtcNow.Year;
            var count = await _context.Prescriptions
                .IgnoreQueryFilters()
                .CountAsync(p => p.CreatedAt.Year == year);

            // Auto-calculate EndDate from StartDate + DurationDays
            var startDate = dto.StartDate ?? DateTime.UtcNow;
            var endDate = startDate.AddDays(dto.DurationDays);

            var prescription = new Prescription
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentId = dto.AppointmentId,
                MedicalRecordId = dto.MedicalRecordId,
                MedicationName = dto.MedicationName,
                Dosage = dto.Dosage,
                Frequency = dto.Frequency,
                DurationDays = dto.DurationDays,
                Instructions = dto.Instructions,
                Notes = dto.Notes,
                PrescribedDate = DateTime.UtcNow,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                ReferenceId = ReferenceIdGenerator.Generate("RX", year, count + 1)
            };

            var created = await _prescriptionRepository.AddAsync(prescription);
            var withDetails = await _prescriptionRepository.GetPrescriptionWithDetailsAsync(created.Id);
            return MapToDto(withDetails);
        }

        public async Task<PrescriptionDto> DeactivatePrescriptionAsync(int prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetPrescriptionWithDetailsAsync(prescriptionId);
            if (prescription == null)
                throw new KeyNotFoundException("Prescription not found");
            if (!prescription.IsActive)
                throw new InvalidOperationException("Prescription is already inactive");

            prescription.IsActive = false;
            await _prescriptionRepository.UpdateAsync(prescription);
            return MapToDto(prescription);
        }

        public async Task DeletePrescriptionAsync(int prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);
            if (prescription == null)
                throw new KeyNotFoundException("Prescription not found");
            await _prescriptionRepository.SoftDeleteAsync(prescription);
        }

        private PrescriptionDto MapToDto(Prescription prescription)
        {
            return new PrescriptionDto
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                PatientName = $"{prescription.Patient.User.FirstName} {prescription.Patient.User.LastName}",
                DoctorId = prescription.DoctorId,
                DoctorName = $"Dr. {prescription.Doctor.User.FirstName} {prescription.Doctor.User.LastName}",
                AppointmentId = prescription.AppointmentId,
                MedicalRecordId = prescription.MedicalRecordId,
                MedicationName = prescription.MedicationName,
                Dosage = prescription.Dosage,
                Frequency = prescription.Frequency,
                DurationDays = prescription.DurationDays,
                Instructions = prescription.Instructions,
                Notes = prescription.Notes,
                PrescribedDate = prescription.PrescribedDate,
                StartDate = prescription.StartDate,
                EndDate = prescription.EndDate,
                IsActive = prescription.IsActive,
                CreatedAt = prescription.CreatedAt
            };
        }
    }
}