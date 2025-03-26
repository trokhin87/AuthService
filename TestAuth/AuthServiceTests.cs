using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DTO;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public AuthServiceTests()
    {
        _jwtServiceMock = new Mock<IJwtService>();
        _configMock = new Mock<IConfiguration>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new System.Uri("http://localhost:5010/")
        };

        _configMock.Setup(c => c["ProxyMicroservice:BaseUrl"]).Returns("http://localhost:5010");

        _authService = new AuthService(_jwtServiceMock.Object, _configMock.Object, new HttpClientFactoryStub(httpClient));
    }

    [Fact]
    public async Task AuthAsync_ValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var loginDto = new LoginDto { Login = "testuser", Password = "password123" };
        var responseDto = new AuthResponseDto { Exists = true };
        var jsonResponse = JsonSerializer.Serialize(responseDto);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        _jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<string>())).Returns("mocked-jwt-token");

        // Act
        var result = await _authService.AuthAsync(loginDto);

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal("mocked-jwt-token", result.Token);
    }

    [Fact]
    public async Task AuthAsync_InvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Login = "invaliduser", Password = "wrongpassword" };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized
            });

        // Act
        var result = await _authService.AuthAsync(loginDto);

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.Equal("Ошибка авторизации", result.Message);
    }

    private class HttpClientFactoryStub : IHttpClientFactory
    {
        private readonly HttpClient _client;
        public HttpClientFactoryStub(HttpClient client) => _client = client;
        public HttpClient CreateClient(string name) => _client;
    }
}
