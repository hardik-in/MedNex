using MedNex_Backend.API.DTOs.Prescription;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Interfaces;
using MedNex_Backend.API.Utilities;

namespace MedNex_Backend.API.Services.Implementations
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IAppointmentRepository appointmentRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<PrescriptionDto> GetPrescriptionByIdAsync(int prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetPrescriptionWithDetailsAsync(prescriptionId);
            if (prescription == null)
                throw new KeyNotFoundException("Prescription not found.");
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

        public async Task<PrescriptionDto> GetPrescriptionByPublicIdAsync(Guid publicId)
        {
            var prescription = await _prescriptionRepository.GetByPublicIdAsync(publicId);
            if (prescription == null)
                throw new KeyNotFoundException("Prescription not found.");

            var withDetails = await _prescriptionRepository.GetPrescriptionWithDetailsAsync(prescription.Id);
            return MapToDto(withDetails!);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByAppointmentPublicIdAsync(Guid appointmentPublicId)
        {
            var appointment = await _appointmentRepository.GetByPublicIdAsync(appointmentPublicId)
                ?? throw new KeyNotFoundException("Appointment not found.");
            var prescriptions = await _prescriptionRepository.GetPrescriptionsByAppointmentAsync(appointment.Id);
            return prescriptions.Select(MapToDto);
        }
        public async Task<IEnumerable<PrescriptionDto>> GetActivePrescriptionsForPatientAsync(int patientId)
        {
            var prescriptions = await _prescriptionRepository.GetActivePrescriptionsForPatientAsync(patientId);
            return prescriptions.Select(MapToDto);
        }

        public async Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto dto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            var year = DateTime.UtcNow.Year;

            var count = await _prescriptionRepository.GetYearlyCountAsync(year);

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
                throw new KeyNotFoundException("Prescription not found.");
            if (!prescription.IsActive)
                throw new InvalidOperationException("Prescription is already inactive.");

            prescription.IsActive = false;
            await _prescriptionRepository.UpdateAsync(prescription);
            return MapToDto(prescription);
        }

        public async Task DeletePrescriptionAsync(int prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);
            if (prescription == null)
                throw new KeyNotFoundException("Prescription not found.");
            await _prescriptionRepository.SoftDeleteAsync(prescription);
        }

        private PrescriptionDto MapToDto(Prescription p)
        {
            return new PrescriptionDto
            {
                PublicId = p.PublicId,
                ReferenceId = p.ReferenceId,
                PatientPublicId = p.Patient.PublicId,
                PatientName = $"{p.Patient.User.FirstName} {p.Patient.User.LastName}",
                DoctorPublicId = p.Doctor.PublicId,
                DoctorName = $"Dr. {p.Doctor.User.FirstName} {p.Doctor.User.LastName}",
                AppointmentPublicId = p.Appointment.PublicId,
                MedicalRecordId = p.MedicalRecordId,
                MedicationName = p.MedicationName,
                Dosage = p.Dosage,
                Frequency = p.Frequency,
                DurationDays = p.DurationDays,
                Instructions = p.Instructions,
                Notes = p.Notes,
                PrescribedDate = p.PrescribedDate,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            };
        }
    }
}