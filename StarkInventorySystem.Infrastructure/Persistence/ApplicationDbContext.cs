using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StarkInventorySystem.Domain.Common;
using StarkInventorySystem.Domain.Entities;
using StarkInventorySystem.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Persistence
{
    /// <summary>
    /// Contexto de base de datos para la aplicación, maneja las conexiones a la base de datos y el mapeo de entidades.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, // Entidad User
        IdentityRole<Guid>,  // Entidad Rol
        Guid> // Type de llave primaria
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para los aggregate roots sólamente (No child entities)
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        // Note: Users, Roles, UserRoles, etc. are inherited from IdentityDbContext
        // Available DbSets from Identity:
        // - Users (ApplicationUser)
        // - Roles (IdentityRole<Guid>)
        // - UserRoles (IdentityUserRole<Guid>)
        // - UserClaims (IdentityUserClaim<Guid>)
        // - UserLogins (IdentityUserLogin<Guid>)
        // - UserTokens (IdentityUserToken<Guid>)
        // - RoleClaims (IdentityRoleClaim<Guid>)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales del modelo pueden ir aquí
            // Esto encuentra todas las clases que implementan IEntityTypeConfiguration<T>
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Personalización de nombres de tablas de Identity (opcional, pero sirve para mayor claridad)
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
            });

            modelBuilder.Entity<IdentityRole<Guid>>(entity =>
            {
                entity.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Tomar todas las entidades con eventos de dominio
            var entitiesWithEvents = ChangeTracker.Entries<Entity>()
                .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            // Tomar todos los eventos de dominio
            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Limpiar los eventos de dominio de las entidades
            foreach (var entity in entitiesWithEvents)
            {
                entity.ClearDomainEvents();
            }

            // Guardar los cambios en la base de datos
            var result = await base.SaveChangesAsync(cancellationToken);

            // TODO: Publicar los eventos de dominio
            // foreach (var domainEvent in domainEvents)
            // {
            //     await _mediator.PublishAsync(domainEvent);
            // }

            return result;
        }
    }
}
