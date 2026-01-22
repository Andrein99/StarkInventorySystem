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
    /// Configuración de la entidad Product para EF Core
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Nombre de tabla
            builder.ToTable("Products");

            // Llave primaria
            builder.HasKey(p => p.Id);

            // Propiedades
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Sku)
                .IsRequired()
                .HasMaxLength(50);

            // Restricción única en Sku
            builder.HasIndex(p => p.Sku)
                .IsUnique();

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.StockQuantity)
                .IsRequired();

            builder.Property(p => p.LowStockThreshold)
            .IsRequired()
            .HasDefaultValue(0);

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt);

            // 🎯 IMPORTANTE: Mapear el objeto de valor Money usando Owned Entity
            // Esto mapea Money como columnas separadas en la tabla Products (No una tabla separada)
            builder.OwnsOne(p => p.Price, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Ignorar eventos de dominio (No se mapean a la base de datos)
            builder.Ignore(p => p.DomainEvents);
        }
    }
}
