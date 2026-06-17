using MedNex_Backend.API.DTOs.Prescription;

namespace MedNex_Backend.API.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> GetPrescriptionByIdAsync(int prescriptionId);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByPatientAsync(int patientId);
        Task<PrescriptionDto> GetPrescriptionByPublicIdAsync(Guid publicId);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByAppointmentPublicIdAsync(Guid appointmentPublicId);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByDoctorAsync(int doctorId);
        Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByAppointmentAsync(int appointmentId);
        Task<IEnumerable<PrescriptionDto>> GetActivePrescriptionsForPatientAsync(int patientId);
        Task<PrescriptionDto> CreatePrescriptionAsync(CreatePrescriptionDto dto);
        Task<PrescriptionDto> DeactivatePrescriptionAsync(int prescriptionId);
        Task DeletePrescriptionAsync(int prescriptionId);
    }
}