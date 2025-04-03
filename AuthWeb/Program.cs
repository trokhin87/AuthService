using AuthWeb.Examples;
using Interfaces;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
// Настройка логирования
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

string dbProxy = String.Empty;
(string, string, string) config=(String.Empty, String.Empty, String.Empty);

if (builder.Environment.IsDevelopment())
{
    // тут настройки для дефолтного запуска без докера
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5082);  
    });
    var configuration = builder.Configuration;
    dbProxy = configuration["ProxyMicroservice:BaseUrl"]?? throw new Exception("");
    config.Item1 = configuration["Jwt:Key"]?? throw new Exception("");
    config.Item2 =configuration["Jwt:Issuer"]?? throw new Exception("");
    config.Item3 = configuration["Jwt:Audience"]?? throw new Exception("");
}
else
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8087);  
    });
    dbProxy = Environment.GetEnvironmentVariable("dbProxy") ?? throw new Exception("");
    config.Item1 = Environment.GetEnvironmentVariable("JwtKey") ?? throw new Exception("");
    config.Item2 = Environment.GetEnvironmentVariable("JwtIssuer") ?? throw new Exception("");
    config.Item3 = Environment.GetEnvironmentVariable("JwtAudience") ?? throw new Exception("");
}

builder.Services.AddHttpClient("ProxyApiClient", client =>
{
    client.BaseAddress = new Uri(dbProxy); 
});


// Добавляем поддержку Swagger с JWT и примерами
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth Microservice API",
        Version = "v1",
        Description = "API для аутентификации пользователей",
    });

    // Добавляем поддержку JWT-токенов в Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите токен в формате: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });

    options.EnableAnnotations(); // Включаем аннотации Swagger
    options.ExampleFilters(); // Добавляем примеры запросов
});

// Добавляем примеры запросов в Swagger
builder.Services.AddSwaggerExamplesFromAssemblyOf<LoginExample>();

// Регистрация сервисов
builder.Services.AddScoped<IAuthService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("ProxyApiClient"); 

    return new AuthService(new JwtService(config),httpClient);
});


builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Microservice API v1");
});
app.UseAuthorization();
app.MapControllers();
Log.Information("Application starting...");
Log.Information($"dbproxy: {dbProxy}");
Log.Information($"key: {config.Item1}");
Log.Information($"issur: {config.Item2}");
Log.Information($"audience: {config.Item3}");
app.Run();