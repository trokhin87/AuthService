using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("YourSuperLongSecretKeyWithAtLeast32Characters");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("your-issuer");
        mockConfig.Setup(c => c["Jwt:Audience"]).Returns("your-audience");

        _jwtService = new JwtService(mockConfig.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        string username = "testuser";

        // Act
        string token = _jwtService.GenerateToken(username);

        // Assert
        Assert.NotNull(token);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Name && c.Value == username);
    }
}