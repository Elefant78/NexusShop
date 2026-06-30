using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NexusShop.WebAPI.Models;
using NexusShop.WebAPI.Security;

namespace NexusShop.WebAPI.Controllers;

/// <summary>
/// Issues JWT access tokens. POST /api/auth/login with a demo user
/// (admin / Admin123!) to obtain an Admin token for the write endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly DemoUserStore _users;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(DemoUserStore users, ITokenService tokenService, IOptions<JwtSettings> jwtSettings)
    {
        _users = users;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>Authenticates a demo user and returns a signed JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
    {
        var user = _users.Find(request.Username, request.Password);
        if (user is null)
            return Unauthorized(new { title = "Invalid username or password." });

        var token = _tokenService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresInMinutes = _jwtSettings.ExpiryMinutes
        });
    }
}
