using DoctorPatientApp.API.Models.Entities;

namespace DoctorPatientApp.API.Repositories.Interfaces
{
    public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord>
    {
        Task<MedicalRecord> GetMedicalRecordWithDetailsAsync(int recordId);
        Task<IEnumerable<MedicalRecord>> GetRecordsByPatientAsync(int patientId);
        Task<IEnumerable<MedicalRecord>> GetRecordsByDoctorAsync(int doctorId);
        Task<MedicalRecord> GetRecordByAppointmentAsync(int appointmentId);
    }
}