using System.Net.Http.Json;
using System.Text.Json;
using DTO;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public AuthService(IJwtService jwtService, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _jwtService = jwtService;
        _config = config;
        _httpClient = httpClientFactory.CreateClient("ProxyClient");
        _baseUrl = _config["ProxyMicroservice:BaseUrl"];
    }


    public async Task<AuthResultDto> AuthAsync(LoginDto loginDto)
    {
        Log.Information("Попытка авторизации пользователя: {@LoginDto}", loginDto);

        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Auth/login", loginDto);
        if (!response.IsSuccessStatusCode)
        {
            Log.Warning("Ошибка при запросе к Proxy API: {StatusCode}", response.StatusCode);
            return new AuthResultDto { IsAuthenticated = false, Message = "Ошибка авторизации" };
        }

        var responseData = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

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