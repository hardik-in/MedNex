using DoctorPatientApp.API.Data;
using DoctorPatientApp.API.DTOs.MedicalRecord;
using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Repositories.Interfaces;
using DoctorPatientApp.API.Services.Interfaces;
using DoctorPatientApp.API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Services.Implementations
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ApplicationDbContext _context;

        public MedicalRecordService(
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            ApplicationDbContext context)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _appointmentRepository = appointmentRepository;
            _context = context;
        }

        public async Task<MedicalRecordDto> GetMedicalRecordByIdAsync(int recordId)
        {
            var record = await _medicalRecordRepository.GetMedicalRecordWithDetailsAsync(recordId);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found");
            return MapToDto(record);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetRecordsByPatientAsync(int patientId)
        {
            var records = await _medicalRecordRepository.GetRecordsByPatientAsync(patientId);
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetRecordsByDoctorAsync(int doctorId)
        {
            var records = await _medicalRecordRepository.GetRecordsByDoctorAsync(doctorId);
            return records.Select(MapToDto);
        }

        public async Task<MedicalRecordDto> GetRecordByAppointmentAsync(int appointmentId)
        {
            var record = await _medicalRecordRepository.GetRecordByAppointmentAsync(appointmentId);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found for this appointment");
            return MapToDto(record);
        }

        public async Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto dto)
        {
            // Validate appointment exists
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            // Prevent duplicate record for same appointment
            var existing = await _medicalRecordRepository.GetRecordByAppointmentAsync(dto.AppointmentId);
            if (existing != null)
                throw new InvalidOperationException("A medical record already exists for this appointment");

            // ── MedicalRecord ReferenceId ─────────────────────────────────
            var year = DateTime.UtcNow.Year;
            var count = await _context.MedicalRecords
                .IgnoreQueryFilters()
                .CountAsync(m => m.CreatedAt.Year == year);

            var record = new MedicalRecord
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentId = dto.AppointmentId,
                Diagnosis = dto.Diagnosis,
                Symptoms = dto.Symptoms,
                Treatment = dto.Treatment,
                DoctorNotes = dto.DoctorNotes,
                LabTestResults = dto.LabTestResults,
                Temperature = dto.Temperature,
                BloodPressureSystolic = dto.BloodPressureSystolic,
                BloodPressureDiastolic = dto.BloodPressureDiastolic,
                HeartRate = dto.HeartRate,
                Weight = dto.Weight,
                Height = dto.Height,
                Recommendations = dto.Recommendations,
                FollowUpDate = dto.FollowUpDate,
                ReferenceId = ReferenceIdGenerator.Generate("MED", year, count + 1)
            };

            var created = await _medicalRecordRepository.AddAsync(record);
            var withDetails = await _medicalRecordRepository.GetMedicalRecordWithDetailsAsync(created.Id);
            return MapToDto(withDetails);
        }

        public async Task<MedicalRecordDto> UpdateMedicalRecordAsync(int recordId, CreateMedicalRecordDto dto)
        {
            var record = await _medicalRecordRepository.GetMedicalRecordWithDetailsAsync(recordId);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found");

            // Only update fields that are provided
            if (!string.IsNullOrEmpty(dto.Diagnosis))
                record.Diagnosis = dto.Diagnosis;
            if (!string.IsNullOrEmpty(dto.Symptoms))
                record.Symptoms = dto.Symptoms;
            if (!string.IsNullOrEmpty(dto.Treatment))
                record.Treatment = dto.Treatment;
            if (!string.IsNullOrEmpty(dto.DoctorNotes))
                record.DoctorNotes = dto.DoctorNotes;
            if (!string.IsNullOrEmpty(dto.LabTestResults))
                record.LabTestResults = dto.LabTestResults;
            if (dto.Temperature.HasValue)
                record.Temperature = dto.Temperature;
            if (dto.BloodPressureSystolic.HasValue)
                record.BloodPressureSystolic = dto.BloodPressureSystolic;
            if (dto.BloodPressureDiastolic.HasValue)
                record.BloodPressureDiastolic = dto.BloodPressureDiastolic;
            if (dto.HeartRate.HasValue)
                record.HeartRate = dto.HeartRate;
            if (dto.Weight.HasValue)
                record.Weight = dto.Weight;
            if (dto.Height.HasValue)
                record.Height = dto.Height;
            if (!string.IsNullOrEmpty(dto.Recommendations))
                record.Recommendations = dto.Recommendations;
            if (dto.FollowUpDate.HasValue)
                record.FollowUpDate = dto.FollowUpDate;

            await _medicalRecordRepository.UpdateAsync(record);
            return MapToDto(record);
        }

        public async Task DeleteMedicalRecordAsync(int recordId)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(recordId);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found");
            await _medicalRecordRepository.SoftDeleteAsync(record);
        }

        private MedicalRecordDto MapToDto(MedicalRecord record)
        {
            return new MedicalRecordDto
            {
                Id = record.Id,
                PatientId = record.PatientId,
                PatientName = $"{record.Patient.User.FirstName} {record.Patient.User.LastName}",
                DoctorId = record.DoctorId,
                DoctorName = $"Dr. {record.Doctor.User.FirstName} {record.Doctor.User.LastName}",
                AppointmentId = record.AppointmentId,
                AppointmentDate = record.Appointment.AppointmentDate,
                Diagnosis = record.Diagnosis,
                Symptoms = record.Symptoms,
                Treatment = record.Treatment,
                DoctorNotes = record.DoctorNotes,
                LabTestResults = record.LabTestResults,
                Temperature = record.Temperature,
                BloodPressureSystolic = record.BloodPressureSystolic,
                BloodPressureDiastolic = record.BloodPressureDiastolic,
                HeartRate = record.HeartRate,
                Weight = record.Weight,
                Height = record.Height,
                Recommendations = record.Recommendations,
                FollowUpDate = record.FollowUpDate,
                CreatedAt = record.CreatedAt
            };
        }
    }
}