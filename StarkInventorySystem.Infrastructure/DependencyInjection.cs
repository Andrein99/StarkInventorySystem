using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.Identity;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Application.Interfaces.Services;
using StarkInventorySystem.Infrastructure.Identity.Configuration;
using StarkInventorySystem.Infrastructure.Identity.Entities;
using StarkInventorySystem.Infrastructure.Identity.Handlers.Login;
using StarkInventorySystem.Infrastructure.Identity.Handlers.Register;
using StarkInventorySystem.Infrastructure.Identity.Services;
using StarkInventorySystem.Infrastructure.Persistence;
using StarkInventorySystem.Infrastructure.Persistence.Repositories;
using StarkInventorySystem.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Registrar el DbContext con SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));


            // Configuraciones de ASP.NET Core Identity
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                // Configuraciones de contraseña
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequiredLength = 8;

                // Configuraciones del lockout (protección contra ataques de fuerza bruta)
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Configuraciones de usuario
                options.User.RequireUniqueEmail = true;

                // Configuraciones de inicio de sesión
                options.SignIn.RequireConfirmedEmail = false; // Poner como true en producción
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configuración de JWT
            var jwtSettings = new JwtSettings();
            configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // Poner como true en producción
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // ===== OpenAPI Configuration with JWT Security Scheme =====
            //services.AddOpenApi(options =>
            //{
            //    options.AddDocumentTransformer((document, context, cancellationToken) =>
            //    {
            //        // Add security scheme for JWT Bearer tokens
            //        document.Components ??= new();
            //        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

            //        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            //        {
            //            Type = SecuritySchemeType.Http,
            //            Scheme = "bearer",
            //            BearerFormat = "JWT",
            //            Description = "Enter your JWT token in the format: Bearer {token}"
            //        };

            //        // Apply security requirement globally to all endpoints
            //        document.SecurityRequirements = new List<OpenApiSecurityRequirement>
            //        {
            //            new OpenApiSecurityRequirement
            //            {
            //                {
            //                    new OpenApiSecurityScheme
            //                    {
            //                        Reference = new OpenApiReference
            //                        {
            //                            Type = ReferenceType.SecurityScheme,
            //                            Id = "Bearer"
            //                        }
            //                    },
            //                    Array.Empty<string>()
            //                }
            //            }
            //        };

            //        return Task.CompletedTask;
            //    });
            //});


            // Políticas de autorización
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
                options.AddPolicy("ManageInventory", policy => policy.RequireRole("Admin", "InventoryManager"));
                options.AddPolicy("ManageOrders", policy => policy.RequireRole("Admin", "OrderManager"));
            });

            // Registrar repositorios
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Registrar UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Registrar servicios
            services.AddScoped<ICurrentUserService, CurrentUserService>();


            // ===== Identity Services =====
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // ===== CRITICAL: Register Authentication Command Handlers =====
            // These handlers are in Infrastructure (not Application), so we register them manually
            services.AddScoped<IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>,
                               RegisterCommandHandler>();

            services.AddScoped<IRequestHandler<LoginCommand, Result<AuthenticationResponse>>,
                               LoginCommandHandler>();

            // Registrar HttpContextAccessor (Necesario para CurrentUserService)
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
