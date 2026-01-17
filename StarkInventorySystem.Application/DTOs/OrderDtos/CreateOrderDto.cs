using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.DTOs.OrderDtos
{
    /// <summary>
    /// DTO para crear una nueva orden.
    /// </summary>
    public class CreateOrderDto
    {
        public Guid CustomerId { get; set; }
        public AddressDto ShippingAddress { get; set; }
        public List<CreateOrderItemDto> Items { get; set; }
    }
}
