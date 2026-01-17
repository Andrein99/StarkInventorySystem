using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Queries.GetOrderById
{
    /// <summary>
    /// Consulta para obtener un pedido por su ID.
    /// </summary>
    public record GetOrderByIdQuery : IRequest<Result<OrderDto>>
    {
        public Guid OrderId { get; init; }
        public GetOrderByIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
