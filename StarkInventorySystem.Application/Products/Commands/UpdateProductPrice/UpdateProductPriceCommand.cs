using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.UpdateProductPrice
{
    /// <summary>
    /// Comando para actualizar el precio de un producto.
    /// </summary>
    public record UpdateProductPriceCommand : IRequest<Result>
    {
        public Guid ProductId { get; init; }
        public decimal NewPrice { get; init; }
        public string Currency { get; init; }
    }
}
