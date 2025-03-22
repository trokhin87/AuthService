using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DTO;
using Interfaces;
using Serilog;

[ApiController]
[Route("api/Auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("")]
    public async Task<IActionResult> CheckUserExists([FromBody] LoginDto loginDto)
    {
        Log.Information("Запрос на авторизацию пользователя: {@LoginDto}", loginDto);

        var authResult = await _authService.AuthAsync(loginDto);

        if (!authResult.IsAuthenticated)
        {
            return Unauthorized(new { message = authResult.Message });
        }

        return Ok(new { token = authResult.Token });
    }

}