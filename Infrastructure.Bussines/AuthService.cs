using System.Net.Http.Json;
using System.Text.Json;
using DTO;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly HttpClient _httpClient;
    public AuthService(IJwtService jwtService, HttpClient httpClient)
    {
        _jwtService = jwtService;
        _httpClient = httpClient;
    }


    public async Task<AuthResultDto> AuthAsync(LoginDto loginDto)
    {
        Log.Information("Попытка авторизации пользователя: {@LoginDto}", loginDto);

        var response = await _httpClient.PostAsJsonAsync($"/api/Auth/login", loginDto);
        if (!response.IsSuccessStatusCode)
        {
            Log.Warning("Ошибка при запросе к Proxy API: {StatusCode}", response.StatusCode);
            return new AuthResultDto { IsAuthenticated = false, Message = "Ошибка авторизации" };
        }

        var responseData = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        if (responseData == null)
        {
            Log.Error("Ответ от Proxy API пришёл пустым или невалидным.");
            return new AuthResultDto { IsAuthenticated = false, Message = "Ошибка авторизации" };
        }
        if (responseData?.Exists == true)
        {
            var token = _jwtService.GenerateToken(loginDto.Login);
            Log.Information("Успешная аутентификация: {Login}", loginDto.Login);
            return new AuthResultDto { IsAuthenticated = true, Token = token };
        }

        Log.Warning("Неудачная аутентификация: {Login}", loginDto.Login);
        return new AuthResultDto { IsAuthenticated = false, Message = "Неправильный логин или пароль" };
    }

}