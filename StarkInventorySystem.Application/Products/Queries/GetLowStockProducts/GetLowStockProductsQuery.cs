using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Queries.GetLowStockProducts
{
    /// <summary>
    /// Consulta para obtener todos los productos con bajo stock.
    /// </summary>
    public record GetLowStockProductsQuery : IRequest<Result<List<ProductDto>>>
    {
        // No se necesitan propiedades, porque la consulta trae todos los productos con bajo stock
    }
}
