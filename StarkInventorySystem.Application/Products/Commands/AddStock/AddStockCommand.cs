using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.AddStock
{
    /// <summary>
    /// Comando para agregar stock a un producto existente.
    /// </summary>
    public class AddStockCommand : IRequest<Result>
    {
        public Guid ProductId { get; init; }
        public int QuantityToAdd { get; init; }

        public AddStockCommand(Guid productId, int quantityToAdd)
        {
            ProductId = productId;
            QuantityToAdd = quantityToAdd;
        }
    }
}
