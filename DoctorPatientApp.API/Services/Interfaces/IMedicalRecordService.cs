using DoctorPatientApp.API.DTOs.MedicalRecord;

namespace DoctorPatientApp.API.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDto> GetMedicalRecordByIdAsync(int recordId);
        Task<IEnumerable<MedicalRecordDto>> GetRecordsByPatientAsync(int patientId);
        Task<IEnumerable<MedicalRecordDto>> GetRecordsByDoctorAsync(int doctorId);
        Task<MedicalRecordDto> GetRecordByAppointmentAsync(int appointmentId);
        Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto dto);
        Task<MedicalRecordDto> UpdateMedicalRecordAsync(int recordId, CreateMedicalRecordDto dto);
        Task DeleteMedicalRecordAsync(int recordId);
    }
}