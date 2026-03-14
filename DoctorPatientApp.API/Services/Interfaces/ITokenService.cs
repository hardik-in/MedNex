using DoctorPatientApp.API.Models.Entities;

namespace DoctorPatientApp.API.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        int? ValidateToken(string token);
    }
}