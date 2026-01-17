using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.CreateProduct
{
    /// <summary>
    /// Comando para crear un nuevo producto.
    /// </summary>
    public record CreateProductCommand : IRequest<Result<Guid>>
    {
        public string Name { get; init; }
        public string Sku { get; init; }
        public decimal Price { get; init; }
        public string Currency { get; init; }
        public string Description { get; init; }
        public int? LowStockThreshold { get; init; }

    }
}
