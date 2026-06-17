using System.ComponentModel.DataAnnotations;

namespace MedNex_Backend.API.DTOs.Doctor
{
    public class UpdateDoctorDto
    {
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Specialization { get; set; }

        public DateTime? CareerStartDate { get; set; }

        [MaxLength(200)]
        public string? Qualifications { get; set; }

        [MaxLength(500)]
        public string Bio { get; set; }

        public decimal? ConsultationFee { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
        public Guid? AssignedAdminId { get; set; }
        public string Email { get; set; }
        public Guid? AssignedAdminPublicId { get; set; }

    }
}
