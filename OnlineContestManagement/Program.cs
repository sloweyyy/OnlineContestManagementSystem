using OnlineContestManagement;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using OnlineContestManagement.Data.Models;
using DotNetEnv;

public class Program
{
    public static void Main(string[] args)
    {
        // Manually load .env file if it exists
        if (File.Exists(".env"))
        {
            Env.Load(".env");
        }

        var builder = WebApplication.CreateBuilder(args);

        // Add configuration sources with priority
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Validate critical configuration
        ValidateConfiguration(builder.Configuration);

        // Debug configuration values
        Console.WriteLine($"JWT Secret Key: {builder.Configuration["JWT_SECRET_KEY"]}");
        Console.WriteLine($"JWT Issuer: {builder.Configuration["JWT_ISSUER"]}");
        Console.WriteLine($"JWT Audience: {builder.Configuration["JWT_AUDIENCE"]}");
        Console.WriteLine($"Google Client ID: {builder.Configuration["GOOGLE_CLIENT_ID"]}");
        Console.WriteLine($"Google Client Secret: {builder.Configuration["GOOGLE_CLIENT_SECRET"]}");
        Console.WriteLine($"MongoDB Connection String: {builder.Configuration["MONGODB_CONNECTIONSTRING"]}");

        // Register services
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IContestRegistrationRepository, ContestRegistrationRepository>();
        builder.Services.AddScoped<IContestRegistrationService, ContestRegistrationService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();

        //Set Policy
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));
        });

        // Run Startup to configure services and middleware
        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();
        startup.Configure(app, builder.Environment);

        app.Run();
    }

    private static void ValidateConfiguration(IConfiguration configuration)
    {
        var criticalVariables = new[]
        {
            "JWT_SECRET_KEY",
            "JWT_ISSUER",
            "JWT_AUDIENCE",
            "JWT_EXPIRY_MINUTES",
            "JWT_REFRESH_TOKEN_EXPIRY_DAYS",
            "GOOGLE_CLIENT_ID",
            "GOOGLE_CLIENT_SECRET",
            "MONGODB_CONNECTIONSTRING",
            "MONGODB_DATABASE_NAME",
            "SMTP_HOST",
            "SMTP_PORT",
            "SMTP_USERNAME",
            "SMTP_PASSWORD",
            "SMTP_USE_SSL",
            "SMTP_FROM_NAME",
            "PAYOS_CLIENT_ID",
            "PAYOS_API_KEY",
            "PAYOS_CHECKSUM_KEY"
        };

        foreach (var variable in criticalVariables)
        {
            var value = configuration[variable];
            if (string.IsNullOrWhiteSpace(value))
            {
                Console.Error.WriteLine($"Critical configuration variable missing: {variable}");
                throw new InvalidOperationException($"Missing required configuration: {variable}");
            }
        }

        // Additional parsing validation for numeric values
        try
        {
            int.Parse(configuration["JWT_EXPIRY_MINUTES"]);
            int.Parse(configuration["JWT_REFRESH_TOKEN_EXPIRY_DAYS"]);
            int.Parse(configuration["SMTP_PORT"]);
        }
        catch (FormatException ex)
        {
            Console.Error.WriteLine($"Invalid numeric configuration: {ex.Message}");
            throw;
        }
    }
}