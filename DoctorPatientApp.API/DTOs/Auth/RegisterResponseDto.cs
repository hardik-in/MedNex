namespace DoctorPatientApp.API.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
    }
}