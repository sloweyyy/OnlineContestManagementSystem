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
        var builder = WebApplication.CreateBuilder(args);

        // Load .env file first
        DotNetEnv.Env.Load();

        // Add configuration sources
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

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

        // Run Startup to configure services and middleware
        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();
        startup.Configure(app, builder.Environment);

        app.Run();
    }
}