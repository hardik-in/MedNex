using MedNex_Backend.API.DTOs.Auth;
using MedNex_Backend.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedNex_Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // POST api/auth/login
        // Returns both an access token (JWT) and a refresh token.
        // Store the refresh token securely — use it only to call /refresh.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                var response = await _authService.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for {Email}", loginRequest.Email);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST api/auth/register
        // Public — Patient and Admin (with code) self-registration.
        // Doctor accounts cannot be created here.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            try
            {
                var response = await _authService.RegisterAsync(registerRequest);
                return StatusCode(201, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for {Email}", registerRequest.Email);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST api/auth/refresh
        // Exchange a valid refresh token for a new access token + new refresh token.
        // Call this when your access token expires (or just before, using ExpiresAt).
        // The old refresh token is immediately revoked — store the new one.
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var response = await _authService.RefreshAsync(request.RefreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Invalid, expired, or already-used refresh token
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token refresh");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST api/auth/logout
        // Revokes the provided refresh token — true logout.
        // The access token lives until natural expiry but cannot be renewed.
        // Client should delete both tokens from storage after calling this.
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            try
            {
                await _authService.LogoutAsync(request.RefreshToken);
                // Always return 204 — even if token was already invalid.
                // Logout should never fail from the user's perspective.
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during logout");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}