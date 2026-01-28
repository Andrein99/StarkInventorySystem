using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarkInventorySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración de la entidad Order para EF Core
    /// </summary>
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Nombre de tabla
            builder.ToTable("Orders");

            // Llave primaria
            builder.HasKey(o => o.Id);

            // Propiedades
            builder.Property(o => o.CustomerId)
                .IsRequired();

            // Índice en CustomerId para consultas rápidas por cliente
            builder.HasIndex(o => o.CustomerId);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>() // Store enum as string in database
                .HasMaxLength(20);

            // Índice en status para consultas frecuentes del estado de la orden
            builder.HasIndex(o => o.Status);

            // Timestamps
            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.ConfirmedAt);
            builder.Property(o => o.ShippedAt);
            builder.Property(o => o.DeliveredAt);
            builder.Property(o => o.CancelledAt);

            builder.Property(o => o.CancellationReason)
                .HasMaxLength(500)
                .IsRequired(false); // Permite tener valores nulos - Sólo se asigna el token cuando cancelada

            // 🎯 Mapea el objecto de valor Shipping Address
            builder.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("ShippingStreet")
                    .HasMaxLength(200)
                    .IsRequired();

                address.Property(a => a.City)
                    .HasColumnName("ShippingCity")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.State)
                    .HasColumnName("ShippingState")
                    .HasMaxLength(50)
                    .IsRequired();

                address.Property(a => a.PostalCode)
                    .HasColumnName("ShippingPostalCode")
                    .HasMaxLength(20)
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("ShippingCountry")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // 🎯 Mapea el objeto de valor Total (Money)
            builder.OwnsOne(o => o.Total, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("TotalAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("TotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // 🎯 IMPORTANTE: La relación entre Order y OrderItems es one-to-many
            // Order es el aggregate root, OrderItems son child entities
            builder.HasMany<OrderItem>("_items") // Access private field
                .WithOne() // OrderItem doesn't have navigation back to Order
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Borra los items si se borra la orden

            // Expone items como una propiedad de navegación
            builder.Navigation("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_items");

            builder.Ignore(o => o.Items); // Ignora la propiedad de solo lectura

            // Ignora eventos de dominio
            builder.Ignore(o => o.DomainEvents);
        }
    }
}
