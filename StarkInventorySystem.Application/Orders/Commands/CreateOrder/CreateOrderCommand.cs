using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Commands.CreateOrder
{
    /// <summary>
    /// Comando para crear una nueva orden.
    /// </summary>
    public record CreateOrderCommand : IRequest<Result<Guid>>
    {
        public Guid CustomerId { get; init; }
        public AddressDto ShippingAddress { get; init; }
        public List<CreateOrderItemDto> Items { get; init; }
    }
}
