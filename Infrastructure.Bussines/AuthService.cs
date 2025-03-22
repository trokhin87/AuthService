using System.Net.Http.Json;
using System.Text.Json;
using DTO;
using Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Bussines;

public class AuthService:IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;
    private string? _baseUrl;
    private readonly HttpClient _httpClient;

    public AuthService(IJwtService jwtService, IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _jwtService = jwtService;
        _baseUrl = _config["ProxyMicroservice:BaseUrl"];
        _httpClient = httpClient;

    }
    public async Task<string> AuthAsync(LoginDto loginDto)
    {
        var response= await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Auth/login", loginDto);
        if (!response.IsSuccessStatusCode) return null;
        var responseData=JsonSerializer.Deserialize<AuthResponseDto>(await response.Content.ReadAsStringAsync())?.Exists??false;
        return responseData ? _jwtService.GenerateToken(loginDto.Login):null;
    }
}