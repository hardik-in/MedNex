using MedNex_Backend.API.DTOs.MedicalRecord;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Repositories.Implementations;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Interfaces;
using MedNex_Backend.API.Utilities;

namespace MedNex_Backend.API.Services.Implementations
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;

        public MedicalRecordService(
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<MedicalRecordDto> GetMedicalRecordByIdAsync(int recordId)
        {
            var record = await _medicalRecordRepository.GetMedicalRecordWithDetailsAsync(recordId);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found.");
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
        public async Task<MedicalRecordDto> GetMedicalRecordByPublicIdAsync(Guid publicId)
        {
            var record = await _medicalRecordRepository.GetByPublicIdAsync(publicId);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found.");

            var withDetails = await _medicalRecordRepository.GetMedicalRecordWithDetailsAsync(record.Id);
            return MapToDto(withDetails!);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetRecordsByPatientPublicIdAsync(Guid patientPublicId)
        {
            var patient = await _patientRepository.GetByPublicIdAsync(patientPublicId)
                ?? throw new KeyNotFoundException("Patient not found.");
            var records = await _medicalRecordRepository.GetRecordsByPatientAsync(patient.Id);
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetRecordsByDoctorPublicIdAsync(Guid doctorPublicId)
        {
            var doctor = await _doctorRepository.GetByPublicIdAsync(doctorPublicId)
                ?? throw new KeyNotFoundException("Doctor not found.");
            var records = await _medicalRecordRepository.GetRecordsByDoctorAsync(doctor.Id);
            return records.Select(MapToDto);
        }

        public async Task<MedicalRecordDto> GetRecordByAppointmentPublicIdAsync(Guid appointmentPublicId)
        {
            var appointment = await _appointmentRepository.GetByPublicIdAsync(appointmentPublicId)
                ?? throw new KeyNotFoundException("Appointment not found.");
            var record = await _medicalRecordRepository.GetRecordByAppointmentAsync(appointment.Id);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found for this appointment.");
            return MapToDto(record);
        }
        public async Task<MedicalRecordDto> GetRecordByAppointmentAsync(int appointmentId)
        {
            var record = await _medicalRecordRepository.GetRecordByAppointmentAsync(appointmentId);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found for this appointment.");
            return MapToDto(record);
        }

        public async Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto dto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            var existing = await _medicalRecordRepository.GetRecordByAppointmentAsync(dto.AppointmentId);
            if (existing != null)
                throw new InvalidOperationException("A medical record already exists for this appointment.");

            var year = DateTime.UtcNow.Year;

            var count = await _medicalRecordRepository.GetYearlyCountAsync(year);

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

        public async Task<MedicalRecordDto> UpdateMedicalRecordAsync(int recordId, UpdateMedicalRecordDto dto)
        {
            var record = await _medicalRecordRepository.GetMedicalRecordWithDetailsAsync(recordId);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found.");

            if (dto.Diagnosis != null) record.Diagnosis = dto.Diagnosis;
            if (dto.Symptoms != null) record.Symptoms = dto.Symptoms;
            if (dto.Treatment != null) record.Treatment = dto.Treatment;
            if (dto.DoctorNotes != null) record.DoctorNotes = dto.DoctorNotes;
            if (dto.LabTestResults != null) record.LabTestResults = dto.LabTestResults;
            if (dto.Temperature.HasValue) record.Temperature = dto.Temperature;
            if (dto.BloodPressureSystolic.HasValue) record.BloodPressureSystolic = dto.BloodPressureSystolic;
            if (dto.BloodPressureDiastolic.HasValue) record.BloodPressureDiastolic = dto.BloodPressureDiastolic;
            if (dto.HeartRate.HasValue) record.HeartRate = dto.HeartRate;
            if (dto.Weight.HasValue) record.Weight = dto.Weight;
            if (dto.Height.HasValue) record.Height = dto.Height;
            if (dto.Recommendations != null) record.Recommendations = dto.Recommendations;
            if (dto.FollowUpDate.HasValue) record.FollowUpDate = dto.FollowUpDate;

            await _medicalRecordRepository.UpdateAsync(record);
            return MapToDto(record);
        }

        public async Task DeleteMedicalRecordAsync(int recordId)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(recordId);
            if (record == null)
                throw new KeyNotFoundException("Medical record not found.");
            await _medicalRecordRepository.SoftDeleteAsync(record);
        }

        private MedicalRecordDto MapToDto(MedicalRecord record)
        {
            return new MedicalRecordDto
            {
                PublicId = record.PublicId,
                ReferenceId = record.ReferenceId,
                PatientPublicId = record.Patient.PublicId,
                PatientName = $"{record.Patient.User.FirstName} {record.Patient.User.LastName}",
                DoctorPublicId = record.Doctor.PublicId,
                DoctorName = $"Dr. {record.Doctor.User.FirstName} {record.Doctor.User.LastName}",
                AppointmentPublicId = record.Appointment.PublicId,
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