using StarkInventorySystem.Domain.Common;
using StarkInventorySystem.Domain.DomainExceptions;
using StarkInventorySystem.Domain.Enums;
using StarkInventorySystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.Entities
{
    public sealed class Order : Entity
    {
        private readonly List<OrderItem> _items = new();

        public Guid CustomerId { get; private set; }
        public Address ShippingAddress { get; private set; }
        public OrderStatus Status { get; private set; }
        public Money Total { get; private set; }


        // Timestamps para el ciclo de vida de la orden
        public DateTime CreatedAt { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public DateTime? ShippedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public string CancellationReason { get; private set; }

        // Expone los items de la orden como una colección de solo lectura (Encapsulación)
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        // EF Core necesita un constructor sin parámetros
        private Order() { }

        private Order(
            Guid id,
            Guid customerId,
            Address shippingAddress)
        {
            Id = id;
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
            Status = OrderStatus.Pending;
            Total = Money.Zero("USD");
            CreatedAt = DateTime.UtcNow;
        }

        // Fábrica para crear una nueva instancia de Order con validación.
        public static Order Create(Guid customerId, Address shippingAddress)
        {
            if (customerId == Guid.Empty)
            {
                throw new DomainException("El Id del cliente no puede estar vacío.");
            }

            if (shippingAddress is null)
            {
                throw new DomainException("La dirección de envío no puede ser nula.");
            }

            var order = new Order(Guid.NewGuid(), customerId, shippingAddress);

            // TODO: Agregar evento de dominio: OrderCreated

            return order;
        }

        #region Item Management

        public void AddItem(Product product, int quantity)
        {
            // Guard clauses
            EnsureOrderCanBeModified();

            if (product is null)
            {
                throw new DomainException("El producto no puede ser nulo.");
            }

            if (!product.IsActive)
            {
                throw new DomainException("No se puede agregar un producto inactivo a la orden.");
            }

            if (!product.HasSufficientStock(quantity))
            {
                throw new DomainException($"No hay stock suficiente para el producto {product.Name}. Stock disponible: {product.StockQuantity}, cantidad solicitada: {quantity}");
            }

            // Crear y añadir el item
            var orderItem = OrderItem.Create(Id, product, quantity);
            _items.Add(orderItem);

            // Recalcular el total de la orden
            RecalculateTotal();

            // TODO: Agregar evento de dominio: OrderItemAdded
        }

        public void RemoveItem(Guid orderItemId)
        {
            // Guard clauses
            EnsureOrderCanBeModified();

            var item = _items.FirstOrDefault(i => i.Id == orderItemId);
            if (item is null)
            {
                throw new DomainException("El ítem a eliminar no existe en la orden.");
            }

            if (_items.Count == 1)
            {
                throw new DomainException("No se puede eliminar el último artículo de la orden. Una orden debe tener al menos un artículo.");
            }

            _items.Remove(item);
            RecalculateTotal();
        }

        public void UpdateItemQuantity(Guid orderItemId, int newQuantity)
        {
            EnsureOrderCanBeModified();

            var item = _items.FirstOrDefault(i => i.Id == orderItemId);
            if (item is null)
                throw new DomainException("Order item not found");

            item.UpdateQuantity(newQuantity);
            RecalculateTotal();

            // TODO: Add domain event: OrderItemQuantityUpdated
        }

        #endregion

        #region Order Status Transitions (State Machine)

        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new DomainException("Solo se pueden modificar órdenes en estado pendiente.");
            }

            if (!_items.Any())
            {
                throw new DomainException("No se puede confirmar una orden sin artículos.");
            }

            Status = OrderStatus.Confirmed;
            ConfirmedAt = DateTime.UtcNow;

            // TODO: Agregar evento de dominio: OrderConfirmed (trigger para reservar Stock)
        }

        public void Ship()
        {
            if (Status != OrderStatus.Confirmed)
            {
                throw new DomainException("La orden debe estar confirmada antes de ser enviada.");
            }

            Status = OrderStatus.Shipped;
            ShippedAt = DateTime.UtcNow;

            // TODO: Agregar evento de dominio: OrderShipped (trigger de notificación)
        }

        public void Deliver()
        {
            if (Status != OrderStatus.Shipped)
            {
                throw new DomainException("Solo se pueden entregar órdenes en estado enviado.");
            }

            Status = OrderStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;

            // TODO: Agregar evento de dominio: OrderDelivered
        }

        public void Cancel(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new DomainException("La razón de cancelación no puede estar vacía.");
            }

            if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            {
                throw new DomainException("No se pueden cancelar órdenes que ya han sido enviadas o entregadas.");
            }

            if (Status == OrderStatus.Cancelled)
            {
                throw new DomainException("La orden ya ha sido cancelada.");
            }

            Status = OrderStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason;

            // TODO: Agregar evento de dominio: OrderCancelled (trigger para liberar Stock)
        }

        #endregion

        #region Private Helper Methods

        private void RecalculateTotal()
        {
            if (!_items.Any())
            {
                Total = Money.Zero("USD");
                return;
            }

            // Empieza con cero en el total
            var total = Money.Zero("USD");

            // Suma los subtotales de cada ítem
            foreach (var item in _items)
            {
                total = total.Add(item.Subtotal);
            }

            Total = total;
        }

        private void EnsureOrderCanBeModified()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new DomainException("Solo se pueden modificar órdenes en estado pendiente.");
            }
        }

        #endregion



        #region Query Methods

        // Verifica si la orden puede ser cancelada (está en estado pendiente o confirmado).
        public bool CanBeCancelled() => Status == OrderStatus.Pending || Status == OrderStatus.Confirmed;
        // Verifica si la orden está en un estado final (entregada o cancelada).
        public bool IsFinal() => Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled;
        // Obtiene la cantidad total de ítems en la orden.
        public int GetItemCount() => _items.Sum(i => i.Quantity);

        #endregion
    }
}
