using DoctorPatientApp.API.DTOs.Prescription;

namespace DoctorPatientApp.API.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> GetPrescriptionByIdAsync(int prescriptionId);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByPatientAsync(int patientId);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByDoctorAsync(int doctorId);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByAppointmentAsync(int appointmentId);
        Task<IEnumerable<PrescriptionDto>> GetActivePrescriptionsForPatientAsync(int patientId);
        Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto dto);
        Task<PrescriptionDto> DeactivatePrescriptionAsync(int prescriptionId);
        Task DeletePrescriptionAsync(int prescriptionId);
    }
}