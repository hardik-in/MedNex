using MedNex_Backend.API.Models.Entities;

namespace MedNex_Backend.API.Repositories.Interfaces
{
    public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord>
    {
        Task<MedicalRecord> GetMedicalRecordWithDetailsAsync(int recordId);
        Task<IEnumerable<MedicalRecord>> GetRecordsByPatientAsync(int patientId);
        Task<IEnumerable<MedicalRecord>> GetRecordsByDoctorAsync(int doctorId);
        Task<MedicalRecord> GetRecordByAppointmentAsync(int appointmentId);
    }
}