using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Commands.ShipOrder
{
    /// <summary>
    /// Comando para cambiar de estado confirmado a enviado la orden.
    /// </summary>
    public record ShipOrderCommand : IRequest<Result>
    {
        public Guid OrderId { get; init; }

        public ShipOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
