using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using StarkInventorySystem.Application;
using StarkInventorySystem.Infrastructure;
using StarkInventorySystem.Infrastructure.Identity.Configuration;
using StarkInventorySystem.Infrastructure.Identity.Data;
using StarkInventorySystem.Infrastructure.Identity.Entities;
using StarkInventorySystem.Infrastructure.Persistence;
using StarkInventorySystem.Infrastructure.Services;
using StarkInventorySystem.WebApi.Middleware;
using System.Reflection;
using static System.Net.WebRequestMethods;

// Esta porción de código sirve para hacer debugging. Se mantiene por si hay que hacerlo en el futuro.
//// ===== TEMPORARY: List all loaded assemblies =====
//Console.WriteLine("===== LOADED ASSEMBLIES =====");
//var assemblies = AppDomain.CurrentDomain.GetAssemblies();
//foreach (var asm in assemblies)
//{
//    Console.WriteLine($"  {asm.GetName().Name} - {asm.GetName().Version}");
//}
//Console.WriteLine("============================");

//// ===== TEMPORARY: Try to load types from WebApi =====
//try
//{
//    var webApiAssembly = Assembly.GetExecutingAssembly();
//    Console.WriteLine($"WebApi Assembly: {webApiAssembly.FullName}");

//    var types = webApiAssembly.GetTypes();
//    Console.WriteLine($"Found {types.Length} types in WebApi");

//    foreach (var type in types.Where(t => t.Name.Contains("Controller")))
//    {
//        Console.WriteLine($"  Controller: {type.FullName}");
//    }
//}
//catch (ReflectionTypeLoadException ex)
//{
//    Console.WriteLine("X REFLECTION ERROR LOADING WEBAPI TYPES:");
//    foreach (var loaderEx in ex.LoaderExceptions)
//    {
//        Console.WriteLine($"  - {loaderEx?.Message}");
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"X ERROR: {ex.Message}");
//}
//Console.WriteLine("============================");


var builder = WebApplication.CreateBuilder(args);


// ===== Configuration Setup =====
// User Secrets are automatically loaded in Development environment
// No code change needed - it's built into WebApplicationBuilder!

// Verify JWT settings are loaded properly
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);

if (!jwtSettings.IsValid())
{
    throw new InvalidOperationException(
        "Las configuraciones de JWT son inválidas. " +
        "Asegúrese que JwtSettings:SecretKey está asignada (min 32 caracteres). " +
        "En Development, usar: dotnet user-secrets set \"JwtSettings:SecretKey\" \"YourSecretKey\"");
}


// Add services to the container.
// Add Controllers
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // Add security scheme for JWT
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Pon tu token JWT en el formato: Bearer {token}"
        };

        // Apply security requirement globally
        document.SecurityRequirements = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            }
        };

        return Task.CompletedTask;
    });
});

// Add Application Layer (Mediator, Handlers, Validators)
builder.Services.AddApplication();

// Add Infrastructure Layer (DbContext, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);


// Add CORS (if needed for frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});



var app = builder.Build();

// Middleware para manejar excepciones (catch a todas las excepciones)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Map Scalar UI (modern, interactive API documentation)
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Stark Inventory & Order Management System API")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();


// Popular la base de datos (En etapa de desarrollo solamente)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Asegurarse que la base de datos esté creada
        logger.LogInformation("Asegurándose que la base de datos esté creada y aplicando migraciones si es necesario...");
        await context.Database.MigrateAsync();

        // Popular la base de datos con datos iniciales
        var seeder = new DatabaseSeeder(context, services.GetRequiredService<ILogger<DatabaseSeeder>>());
        await seeder.SeedAsync();

        // Popular data de Identity (Roles, usuario Admin)
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var identityLogger = services.GetRequiredService<ILogger<IdentityDataSeeder>>();
        var identitySeeder = new IdentityDataSeeder(userManager, roleManager, identityLogger);
        await identitySeeder.SeedAsync();



        logger.LogInformation("Base de datos populada satisfactoriamente.");
        logger.LogInformation("Navigate to: https://localhost:7204/scalar/v1 for API documentation");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Un error ocurrió al popular la base de datos");
    }
}

// CORS
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

try
{
    app.MapControllers();
}
catch (ReflectionTypeLoadException ex)
{
    Console.WriteLine("===== LOADER EXCEPTIONS =====");
    foreach (var loaderEx in ex.LoaderExceptions ?? Array.Empty<Exception>())
    {
        Console.WriteLine($"ERROR: {loaderEx?.Message}");
    }
    throw;
}

app.Run();
