using StarkInventorySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.DTOs.OrderDtos
{
    /// <summary>
    /// DTO para transferir datos de una orden en el sistema de inventario.
    /// </summary>
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public AddressDto ShippingAddress { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string CancellationReason { get; set; }
    }
}
