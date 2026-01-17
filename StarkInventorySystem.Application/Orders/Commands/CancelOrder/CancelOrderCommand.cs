using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Commands.CancelOrder
{
    /// <summary>
    /// Comando para cancelar un pedido.
    /// </summary>
    public record CancelOrderCommand : IRequest<Result>
    {
        public Guid OrderId { get; init; }
        public string Reason { get; init; }

        public CancelOrderCommand(Guid orderId, string reason)
        {
            OrderId = orderId;
            Reason = reason;
        }
    }
}
