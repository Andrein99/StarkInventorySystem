using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Application.Interfaces.Services;
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


            // Registrar repositorios
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Registrar UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Registrar servicios
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Registrar HttpContextAccessor (Necesario para CurrentUserService)
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
