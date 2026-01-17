using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.DTOs.OrderDtos
{
    /// <summary>
    /// DTO para transferir datos de un ítem de orden en el sistema de inventario.
    /// </summary>
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Currency { get; set; }
        public decimal Subtotal { get; set; }
    }
}
