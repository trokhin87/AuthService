using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AuthWeb.Examples;
using DTO;
using Interfaces;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

[ApiController]
[Route("api/Auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Авторизация пользователя.
    /// </summary>
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Авторизация", Description = "Проверяет учетные данные пользователя и выдает токен.")]
    [SwaggerRequestExample(typeof(LoginDto), typeof(LoginExample))]
    [SwaggerResponse(200, "Успешная авторизация", typeof(object))]
    [SwaggerResponse(401, "Ошибка авторизации", typeof(object))]
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