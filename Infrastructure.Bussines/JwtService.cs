using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration configuration)
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

        var keyString = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(keyString))
        {
            Log.Fatal("Ключ JWT не задан в конфигурации!");
            throw new InvalidOperationException("Jwt:Key не может быть пустым.");
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        Log.Information("JWT токен сгенерирован успешно");

        return jwt;
    }
}