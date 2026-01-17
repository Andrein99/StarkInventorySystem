using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.ProductDtos;
using StarkInventorySystem.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Queries.GetLowStockProducts
{
    /// <summary>
    /// Manejador para la consulta GetLowStockProductsQuery.
    /// </summary>
    public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, Result<List<ProductDto>>>
    {
        private readonly IProductRepository _productRepository;

        public GetLowStockProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Result<List<ProductDto>>> HandleAsync(
            GetLowStockProductsQuery request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var lowStockProducts = await _productRepository.GetLowStockProductsAsync(cancellationToken);

                // Mapear las entidades Product a ProductDto
                var productDtos = lowStockProducts.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    Price = p.Price.Amount,
                    Currency = p.Price.Currency,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold,
                    IsActive = p.IsActive,
                    IsLowStock = p.IsLowStock(),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                return Result<List<ProductDto>>.Success(productDtos);
            }
            catch (Exception ex)
            {
                return Result<List<ProductDto>>.Failure($"Error al obtener productos con bajo stock: {ex.Message}");
            }
        }
    }
}
