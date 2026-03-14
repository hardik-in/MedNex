namespace DoctorPatientApp.API.DTOs.Doctor
{
    public class DoctorListDto
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string LicenseNumber { get; set; }

        public string Specialization { get; set; }

        public int YearsOfExperience { get; set; }

        public decimal ConsultationFee { get; set; }

        public int? AssignedAdminId { get; set; }

        public string AssignedAdminName { get; set; }

        public bool IsActive { get; set; }
    }
}