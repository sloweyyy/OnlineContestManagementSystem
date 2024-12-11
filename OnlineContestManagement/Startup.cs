using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using OnlineContestManagement.Data.Repositories;
using OnlineContestManagement.Infrastructure.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OnlineContestManagement.Infrastructure;
using System.Security.Claims;
using Net.payOS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace OnlineContestManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration
        {
            get;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "OnlineContestManagement API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
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
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();

            // Configure Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IContestService, ContestService>();
            services.AddScoped<IContestRegistrationService, ContestRegistrationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<INewsService, NewsService>();

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                {
                    if (cookieContext.CookieOptions.SameSite == SameSiteMode.None)
                    {
                        cookieContext.CookieOptions.Secure = true;
                    }
                };
                options.OnDeleteCookie = cookieContext =>
                {
                    if (cookieContext.CookieOptions.SameSite == SameSiteMode.None)
                    {
                        cookieContext.CookieOptions.Secure = true;
                    }
                };
            });

            // Add JWT authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.LoginPath = "/api/Auth/google-login";
                    options.LogoutPath = "/api/Auth/revoke-token";

                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JWT_ISSUER"],
                        ValidAudience = Configuration["JWT_AUDIENCE"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_SECRET_KEY"]))
                    };
                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["GOOGLE_CLIENT_ID"];
                    options.ClientSecret = Configuration["GOOGLE_CLIENT_SECRET"];

                    options.CallbackPath = "/api/Auth/google-response";

                    // Map external claims to internal claims
                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");

                    // Optional: Configure other options
                    options.SaveTokens = true;

                });

            services.AddSingleton(sp =>
            {
                var smtpSettings = new SmtpSettings
                {
                    Host = Configuration["SMTP_HOST"],
                    Port = int.Parse(Configuration["SMTP_PORT"]),
                    Username = Configuration["SMTP_USERNAME"],
                    Password = Configuration["SMTP_PASSWORD"],
                    UseSSL = bool.Parse(Configuration["SMTP_USE_SSL"]),
                    UseTLS = bool.Parse(Configuration.GetValue("SMTP_USE_TLS", "false")),
                    FromName = Configuration["SMTP_FROM_NAME"]
                };
                return smtpSettings;
            });

            services.AddSingleton(sp =>
            {
                var payOS = new PayOSSettings
                {
                    ClientId = Configuration["PAYOS_CLIENT_ID"],
                    ApiKey = Configuration["PAYOS_API_KEY"],
                    ChecksumKey = Configuration["PAYOS_CHECKSUM_KEY"]
                };
                return payOS;
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
            app.UseCookiePolicy();

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