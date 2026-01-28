using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using StarkInventorySystem.Application;
using StarkInventorySystem.Infrastructure;
using StarkInventorySystem.Infrastructure.Persistence;
using StarkInventorySystem.Infrastructure.Services;
using StarkInventorySystem.WebApi.Middleware;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add Controllers
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Application Layer (Mediator, Handlers, Validators)
builder.Services.AddApplication();

// Add Infrastructure Layer (DbContext, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

    /*// Add JWT Authentication to Swagger (optional, for later)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
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
            Array.Empty<string>()
        }
    });*/

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

// CORS
app.UseCors("AllowAll");

// CORS
app.UseCors("AllowAll");

// Authentication & Authorization (add later when implementing JWT)
// app.UseAuthentication();
// app.UseAuthorization();

// app.UseAuthorization();

app.MapControllers();


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

        logger.LogInformation("Base de datos populada satisfactoriamente.");
        logger.LogInformation("Navigate to: https://localhost:7204/scalar/v1 for API documentation");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Un error ocurrió al popular la base de datos");
    }
}

app.Run();
