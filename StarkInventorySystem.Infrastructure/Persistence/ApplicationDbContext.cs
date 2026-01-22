using Microsoft.EntityFrameworkCore;
using StarkInventorySystem.Domain.Common;
using StarkInventorySystem.Domain.Entities;
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
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para los aggregate roots sólamente (No child entities)
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        // Nota: No hay DbSet para OrderItem ya que es una entidad hija de Order

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales del modelo pueden ir aquí
            // Esto encuentra todas las clases que implementan IEntityTypeConfiguration<T>
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
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
