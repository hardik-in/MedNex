using System.ComponentModel.DataAnnotations;

namespace DoctorPatientApp.API.DTOs.Admin
{
    public class UpdateAdminDto
    {
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string Department { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }
    }
}