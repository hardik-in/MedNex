using MedNex_Backend.API.DTOs.MedicalRecord;

namespace MedNex_Backend.API.Services.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDto> GetMedicalRecordByIdAsync(int recordId);
        Task<MedicalRecordDto> GetMedicalRecordByPublicIdAsync(Guid publicId);
        Task<IEnumerable<MedicalRecordDto>> GetRecordsByPatientAsync(int patientId);
        Task<IEnumerable<MedicalRecordDto>> GetRecordsByDoctorAsync(int doctorId);
        Task<MedicalRecordDto> GetRecordByAppointmentAsync(int appointmentId);
        Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto dto);
        Task<MedicalRecordDto> UpdateMedicalRecordAsync(int recordId, UpdateMedicalRecordDto dto);
        Task<IEnumerable<MedicalRecordDto>> GetRecordsByPatientPublicIdAsync(Guid patientPublicId);
        Task<IEnumerable<MedicalRecordDto>> GetRecordsByDoctorPublicIdAsync(Guid doctorPublicId);
        Task<MedicalRecordDto> GetRecordByAppointmentPublicIdAsync(Guid appointmentPublicId);

        Task DeleteMedicalRecordAsync(int recordId);
    }
}