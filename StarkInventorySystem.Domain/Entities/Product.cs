using StarkInventorySystem.Domain.Common;
using StarkInventorySystem.Domain.DomainExceptions;
using StarkInventorySystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.Entities
{
    public sealed class Product : Entity
    {
        public string Name { get; private set; }
        public string Sku { get; private set; }
        public Money Price { get; private set; }
        public string Description { get; private set; }
        public int StockQuantity { get; private set; }
        public int LowStockThreshold { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // EF Core requiere un constructor sin parámetros
        private Product() { }

        private Product(
            Guid id,
            string name,
            string sku,
            Money price,
            string description)
        {
            Id = id;
            Name = name;
            Sku = sku;
            Price = price;
            Description = description;
            StockQuantity = 0;
            LowStockThreshold = 0; // Valor por defecto
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        // Factory method para crear una nueva instancia de Product con validaciones.
        public static Product Create(
            string name,
            string sku,
            Money price,
            string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("El nombre del producto no puede estar vacío.");
            }

            if (string.IsNullOrWhiteSpace(sku))
            {
                throw new DomainException("El SKU del producto no puede estar vacío.");
            }

            if (price is null)
            {
                throw new DomainException("El precio del producto no puede ser nulo.");
            }

            return new Product(Guid.NewGuid(), name, sku, price, description);
        }

        // Método para agregar stock al producto.
        public void AddStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new DomainException("La cantidad a agregar al stock debe ser mayor que cero.");
            }

            StockQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;

            // TODO: Agregar evento de dominio: StockAdded
        }

        // Método para quitar stock del producto.
        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new DomainException("La cantidad a quitar del stock debe ser mayor que cero.");
            }

            if (quantity > StockQuantity)
            {
                throw new DomainException($"No hay stock suficiente. Disponible: {StockQuantity}. Solicitado: {quantity}");
            }

            StockQuantity -= quantity;
            UpdatedAt = DateTime.UtcNow;

            // TODO: Agregar evento de dominio: StockRemoved
            // TODO: Verificar si el stock está por debajo del umbral y generar evento LowStock si es necesario
        }

        // Verifica si hay suficiente stock para una cantidad dada.
        public bool HasSufficientStock(int quantity)
        {
            return StockQuantity >= quantity;
        }

        // Método para actualizar el precio del producto.
        public void UpdatePrice(Money newPrice)
        {
            if (newPrice is null)
            {
                throw new DomainException("El precio del producto no puede ser nulo.");
            }

            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;

            // TODO: Agregar evento de dominio: PriceChanged
        }

        // Método para actualizar el nombre y la descripción del producto.
        public void UpdateInfo(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new DomainException("El nombre del producto no puede estar vacío.");
            }

            Name = name;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        // Método para desactivar producto (soft delete). Los productos no se borran por temas de auditoría, y porque se deben reactivar cuando se consiga el stock o se arregle el problema correspondiente.
        // Regla de negocio: No se pueden vender producto desactivados.
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;

            // TODO: Agregar evento de dominio: ProductDeactivated
        }


        // Método para activar producto.
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;

            // TODO: Agregar evento de dominio: ProductActivated
        }

        // Establece el umbral de bajo stock para el producto.
        public void SetLowStockThreshold(int threshold)
        {
            if (threshold < 0)
            {
                throw new DomainException("El umbral de bajo stock no puede ser negativo.");
            }
            LowStockThreshold = threshold;
            UpdatedAt = DateTime.UtcNow;
        }

        // Verifica si el stock actual está por debajo o igual al umbral de bajo stock.
        public bool IsLowStock()
        {
            return StockQuantity <= LowStockThreshold;
        }
    }
}
