using StarkInventorySystem.Domain.Common;
using StarkInventorySystem.Domain.DomainExceptions;
using StarkInventorySystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.Entities
{
    public sealed class OrderItem : Entity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string ProductSku { get; private set; }
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money Subtotal { get; private set; }

        // EF Core propiedad de navegación (opcional, pero útil)
        // No cargamos esto en la lógica de dominio, pero EF Core lo usa para relaciones.
        private Order _order;

        // EF Core necesita un constructor sin parámetros
        private OrderItem() { }

        private OrderItem(
            Guid id,
            Guid orderId,
            Guid productId,
            string productName,
            string productSku,
            int quantity,
            Money unitPrice)
        {
            Id = id;
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            ProductSku = productSku;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Subtotal = unitPrice.Multiply(quantity);
        }

        // Fábrica para crear una nueva instancia de OrderItem de un producto con validación.
        internal static OrderItem Create(
            Guid orderId,
            Product product,
            int quantity)
        {
            if (product is null)
            {
                throw new DomainException("El producto no puede ser nulo.");
            }

            if (quantity <= 0)
            {
                throw new DomainException("La cantidad de artículos en la orden debe ser un número positivo.");
            }

            if (!product.IsActive)
            {
                throw new DomainException("No se puede agregar un producto inactivo al pedido.");
            }

            if (!product.HasSufficientStock(quantity))
            {
                throw new DomainException($"No hay suficiente stock para el producto {product.Name}. Stock disponible: {product.StockQuantity}, cantidad solicitada: {quantity}.");
            }

            return new OrderItem(
                Guid.NewGuid(),
                orderId,
                product.Id,
                product.Name,
                product.Sku,
                quantity,
                product.Price);
        }

        // Actualiza la cantidad del OrderItem y recalcula el subtotal. Sería usada si queremos permitir cambios en la cantidad.
        internal void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
            {
                throw new DomainException("La nueva cantidad debe ser mayor que cero al actualizar un OrderItem.");
            }
            Quantity = newQuantity;
            Subtotal = UnitPrice.Multiply(newQuantity);
        }
    }
}
