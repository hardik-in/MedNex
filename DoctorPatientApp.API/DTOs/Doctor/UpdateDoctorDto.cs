using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.API.DTOs.Doctor
{
    public class UpdateDoctorDto
    {
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Specialization { get; set; }

        public int? YearsOfExperience { get; set; }

        [MaxLength(200)]
        public string? Qualifications { get; set; }

        [MaxLength(500)]
        public string Bio { get; set; }

        public decimal? ConsultationFee { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
        public int? AssignedAdminId { get; set; }
        public string Email { get; set; }
  
}
    }
