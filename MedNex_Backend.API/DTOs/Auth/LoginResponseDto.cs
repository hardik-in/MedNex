namespace MedNex_Backend.API.DTOs.Auth
{
    public class LoginResponseDto
    {
        // Short-lived JWT — send in Authorization header for every API call
        public string Token { get; set; }

        // Long-lived refresh token — send ONLY to POST /api/auth/refresh
        // Store securely on client (HttpOnly cookie preferred over localStorage)
        public string RefreshToken { get; set; }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

        // When the ACCESS token (JWT) expires — client uses this to know
        // when to call /refresh before the next API call fails with 401.
        public DateTime ExpiresAt { get; set; }

        // Previous login time — shown in UI as "Last seen: X"
        public DateTime? LastLoginAt { get; set; }
    }
}