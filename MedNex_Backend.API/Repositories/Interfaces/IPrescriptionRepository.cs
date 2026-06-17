using MedNex_Backend.API.Models.Entities;

namespace MedNex_Backend.API.Repositories.Interfaces
{
    public interface IPrescriptionRepository : IGenericRepository<Prescription>
    {
        Task<Prescription?> GetPrescriptionWithDetailsAsync(int prescriptionId);
        Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId);
        Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId);
        Task<IEnumerable<Prescription>> GetPrescriptionsByAppointmentAsync(int appointmentId);
        Task<IEnumerable<Prescription>> GetActivePrescriptionsForPatientAsync(int patientId);
    }
}