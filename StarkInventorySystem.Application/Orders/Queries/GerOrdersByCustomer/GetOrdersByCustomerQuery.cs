using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Queries.GerOrdersByCustomer
{
    /// <summary>
    /// Consulta para obtener todas las órdenes de un cliente específico.
    /// </summary>
    public class GetOrdersByCustomerQuery : IRequest<Result<List<OrderDto>>>
    {
        public Guid CustomerId { get; init; }

        public GetOrdersByCustomerQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}
