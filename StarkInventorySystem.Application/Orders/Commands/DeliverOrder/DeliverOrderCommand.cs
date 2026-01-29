using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Commands.DeliverOrder
{
    /// <summary>
    /// Comando para cambiar de estado enviado a entregado de la orden.
    /// </summary>
    public record DeliverOrderCommand : IRequest<Result>
    {
        public Guid OrderId { get; init; }
        public DeliverOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
