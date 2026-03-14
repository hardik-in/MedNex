using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorPatientApp.API.Models.Entities
{
    public class Doctor : BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        public int? AssignedAdminId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; }

        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; }

        [Required]
        public DateTime CareerStartDate { get; set; }

        [NotMapped]
        public int YearsOfExperience => DateTime.UtcNow.Year - CareerStartDate.Year;

        [MaxLength(200)]
        public string Qualifications { get; set; }

        [MaxLength(500)]
        public string Bio { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal ConsultationFee { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("AssignedAdminId")]
        public Admin? AssignedAdmin { get; set; }

        public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}