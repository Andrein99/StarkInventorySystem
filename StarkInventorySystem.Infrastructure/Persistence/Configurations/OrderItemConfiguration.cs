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
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Nombre de tabla
            builder.ToTable("OrderItems");

            // Llave primaria
            builder.HasKey(oi => oi.Id);

            // Llave foránea a Order (Creada en OrderConfiguration)
            builder.Property(oi => oi.OrderId)
                .IsRequired();

            // Index en OrderId para consultas rápidas por orden
            builder.HasIndex(oi => oi.OrderId);


            // Referencia de producto (almacenamos ProductId, no una restricción de llave foránea)
            // Esto se debe a que el producto podría ser eliminado, pero mantenemos el historial de órdenes
            builder.Property(oi => oi.ProductId)
                .IsRequired();

            // Creamos un snapshot de los detalles del producto al momento de la orden
            builder.Property(oi => oi.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(oi => oi.ProductSku)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            // 🎯 Mapeo del objecto de valor UnitPrice (Money)
            builder.OwnsOne(oi => oi.UnitPrice, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("UnitPrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // 🎯 Mapeo del objeto de valor Subtotal (Money)
            builder.OwnsOne(oi => oi.Subtotal, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Subtotal")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("SubtotalCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Ignorar los eventos de dominio
            builder.Ignore(oi => oi.DomainEvents);
        }
    }
}
