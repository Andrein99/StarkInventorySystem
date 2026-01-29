using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Queries.GetAllProducts
{
    /// <summary>
    /// Consulta para obtener todos los productos con paginación y filtro opcional por estado de activación.
    /// </summary>
    public record GetAllProductsQuery : IRequest<Result<PagedResult<ProductDto>>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public bool? IsActive { get; init; } // Filtro opcional por estado de activación
    }
}
