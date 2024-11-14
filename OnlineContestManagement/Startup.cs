using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OnlineContestManagement.Data;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Infrastructure.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using OnlineContestManagement.Infrastructure;

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
