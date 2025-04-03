using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

public class JwtService : IJwtService
{
    private readonly (string, string, string) _config;

    public JwtService((string, string, string) configuration)
    {
        _config = configuration;
    }

    public string GenerateToken(string username)
    {
        Log.Information("Генерация JWT токена для {Username}", username);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username)
        };

        var keyString = _config.Item1?? throw new Exception("");
        if (string.IsNullOrEmpty(keyString))
        {
            Log.Fatal("Ключ JWT не задан в конфигурации!");
            throw new InvalidOperationException("Jwt:Key не может быть пустым.");
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config.Item2?? throw new Exception(""),
            audience: _config.Item3?? throw new Exception(""),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        Log.Information("JWT токен сгенерирован успешно");

        return jwt;
    }
}