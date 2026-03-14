using DoctorPatientApp.API.DTOs.Auth;

namespace DoctorPatientApp.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    }
}