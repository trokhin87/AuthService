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

    [HttpPost("login")]
    public async Task<IActionResult> CheckUserExists([FromBody] LoginDto loginDto)
    {
        Log.Information("Запрос проверки пользователя: {@LoginDto}", loginDto);

        string exists = await _authService.AuthAsync(loginDto);

        Log.Information("Результат проверки пользователя: {Exists}", exists);

        return Ok(new { exists });
    }
}