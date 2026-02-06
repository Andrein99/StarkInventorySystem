using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StarkInventorySystem.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Identity.Data
{
    /// <summary>
    /// Popula la data inicial para Identity (roles y usuario admin).
    /// Debe ser llamado al iniciar la aplicación.
    /// </summary>
    public class IdentityDataSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<IdentityDataSeeder> _logger;

        public IdentityDataSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ILogger<IdentityDataSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Comenzando populación de data de Identity...");

                // Popular roles inicialmente
                await SeedRolesAsync();

                // Popular usuario admin
                await SeedAdminUserAsync();

                _logger.LogInformation("Populación de data de Identity se completó satisfactoriamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió mientras se populaba de data de Identity.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            _logger.LogInformation("Populando roles...");

            string[] roles = { "Admin", "Customer", "InventoryManager", "OrderManager", "WarehouseStaff" };
        
            foreach (var roleName in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new IdentityRole<Guid>
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant(),
                    };

                    var result = await _roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Rol creado: {roleName}");
                    }
                    else
                    {
                        _logger.LogError("Falló la creación del rol {RoleName}: {Errors}",
                            roleName,
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    _logger.LogInformation("El rol ya existe: {RoleName}", roleName);
                }
            }
        }


        private async Task SeedAdminUserAsync()
        {
            _logger.LogInformation("Populando al usuario admin...");

            const string adminEmail = "admin@starkinventory.com";
            const string adminUsername = "admin";
            const string adminPassword = "Admin123!"; // Cambiar en producción

            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // Agregar rol de Admin
                    await _userManager.AddToRoleAsync(adminUser, "Admin");

                    _logger.LogInformation(
                        "Usuario Admin creado satisfactoriamente. Email: {Email}, Username: {Username}",
                        adminEmail,
                        adminUsername);

                    _logger.LogWarning(
                        "⚠️ La contraseña por defecto de admin es 'Admin123!' - ¡CAMBIAR EN PRODUCCIÓN!");
                }
                else
                {
                    _logger.LogError("Falló la creación del usuario admin: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation("El usuario administrador ya existe: {Email}", adminEmail);

                // Asegurar que el admin tiene el rol Admin
                var isInRole = await _userManager.IsInRoleAsync(existingAdmin, "Admin");
                if (!isInRole)
                {
                    await _userManager.AddToRoleAsync(existingAdmin, "Admin");
                    _logger.LogInformation("Se añadió el rol de Admin al usuario existente: {Email}", adminEmail);
                }

            }
        }
    }
}
