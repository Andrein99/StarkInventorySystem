using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Commands.ConfirmOrder
{
    /// <summary>
    /// Comando para confirmar un pedido.
    /// </summary>
    public record ConfirmOrderCommand : IRequest<Result>
    {
        public Guid OrderId { get; init; }

        public ConfirmOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
