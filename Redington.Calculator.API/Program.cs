using FluentValidation;
using Serilog;

using Redington.Calculator.API.Filters;
using Redington.Calculator.API.Middleware;
using Redington.Calculator.API.Validation;
using Redington.Calculator.Domain.Interfaces;
using Redington.Calculator.Domain.Models;
using Redington.Calculator.Infrastructure.Functions;
using Redington.Calculator.Infrastructure.Services;
using Redington.Calculator.API;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/calculator-.log", rollingInterval: RollingInterval.Day);
});

// Add validation filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add FluentValidation
builder.Services.AddScoped<IValidator<CalculationRequest>, CalculationRequestValidator>();

// Add validation filter
builder.Services.AddScoped<ValidationFilter>();

// Register core services (functions + calculation service)
AddCalculatorCore(builder.Services);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Bind the Constants section of appsettings.json
builder.Services.Configure<Constants>(
    builder.Configuration.GetSection("Constants")
);

// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

// Use custom exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting Calculator API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Ending Calculator API");
    Log.CloseAndFlush();
}

static void AddCalculatorCore(IServiceCollection services)
{
    // Register Calculator Core services (functions + calculation service)
    services.AddSingleton<ICalculationFunction, CombinedWithFunction>();
    services.AddSingleton<ICalculationFunction, EitherFunction>();

    // Register calculation service
    services.AddScoped<ICalculationService, CalculationService>();
}