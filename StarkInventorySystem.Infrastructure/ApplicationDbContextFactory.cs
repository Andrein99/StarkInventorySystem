using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StarkInventorySystem.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure
{
    /// <summary>
    /// Design-time factory for creating ApplicationDbContext during migrations.
    /// EF Core uses this to create DbContext when running 'dotnet ef' commands.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json in WebApi project
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../StarkInventorySystem.WebApi"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found in appsettings.json. " +
                    "Make sure appsettings.json exists in the WebApi project.");
            }

            // Create DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
