namespace MedNex_Backend.API.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(int userId, string email, string role, string fullName);
        int? ValidateToken(string token);
        int GetExpirationMinutes();
        string GenerateRefreshToken();
    }
}