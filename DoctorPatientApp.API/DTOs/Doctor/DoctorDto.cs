namespace DoctorPatientApp.API.DTOs.Doctor
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public string Qualifications { get; set; }
        public string Bio { get; set; }
        public decimal ConsultationFee { get; set; }
        public int? AssignedAdminId { get; set; }
        public string AssignedAdminName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}