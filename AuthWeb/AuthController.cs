using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DTO;
using Interfaces;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await _authService.AuthAsync(loginDto);
        if (token == null)
            return Unauthorized(new { message = "Неверный логин или пароль" });

        return Ok(new { token });
    }
}