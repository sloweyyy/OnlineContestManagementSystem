using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Infrastructure.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OnlineContestManagement.Infrastructure;
using System.Security.Claims;

namespace OnlineContestManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OnlineContestManagement API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Add HttpContextAccessor
            services.AddHttpContextAccessor();

            // Configure SmtpClient
            // Configure SmtpClient from environment variables

            // Configure MongoDB
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));
            services.AddSingleton<IMongoClient>(sp =>
            {
                return new MongoClient(Configuration["MONGODB_CONNECTIONSTRING"]);
            });
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(Configuration["MONGODB_DATABASE_NAME"]);
            });

            // Configure Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IContestRepository, ContestRepository>();
            services.AddScoped<IContestRegistrationRepository, ContestRegistrationRepository>();

            // Configure Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IContestService, ContestService>();
            services.AddScoped<IContestRegistrationService, ContestRegistrationService>();
            services.AddScoped<IEmailService, EmailService>();


            // Configure JWT Authentication
            var jwtSettings = new JwtSettings
            {
                SecretKey = Configuration["JWT_SECRET_KEY"],
                Issuer = Configuration["JWT_ISSUER"],
                Audience = Configuration["JWT_AUDIENCE"],
                ExpiryMinutes = int.Parse(Configuration["JWT_EXPIRY_MINUTES"]),
                RefreshTokenExpiryDays = int.Parse(Configuration["JWT_REFRESH_TOKEN_EXPIRY_DAYS"])
            };

            services.Configure<JwtSettings>(options =>
            {
                options.SecretKey = jwtSettings.SecretKey;
                options.Issuer = jwtSettings.Issuer;
                options.Audience = jwtSettings.Audience;
                options.ExpiryMinutes = jwtSettings.ExpiryMinutes;
                options.RefreshTokenExpiryDays = jwtSettings.RefreshTokenExpiryDays;
            });

            // Add JWT authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

            services.AddSingleton(sp =>
            {
                var smtpSettings = new SmtpSettings
                {
                    Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? throw new Exception("SMTP_HOST environment variable not set"),
                    Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? throw new Exception("SMTP_PORT environment variable not set")),
                    Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? throw new Exception("SMTP_USERNAME environment variable not set"),
                    Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new Exception("SMTP_PASSWORD environment variable not set"),
                    UseSSL = bool.Parse(Environment.GetEnvironmentVariable("SMTP_USE_SSL") ?? throw new Exception("SMTP_USE_SSL environment variable not set")),
                    UseTLS = bool.Parse(Environment.GetEnvironmentVariable("SMTP_USE_TLS") ?? "false"),
                    FromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? throw new Exception("SMTP_FROM_NAME environment variable not set")
                };
                return smtpSettings;
            });

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineContestManagement v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
