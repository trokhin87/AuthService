using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Добавляем Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();
app.UseSerilogRequestLogging();
app.Run();