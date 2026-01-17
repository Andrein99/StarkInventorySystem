using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.ProductDtos;
using StarkInventorySystem.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<ProductDto>> HandleAsync(
            GetProductByIdQuery request, 
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    return Result<ProductDto>.Failure($"El producto con ID {request.ProductId} no fue encontrado.");
                }

                // Mapear la entidad Product a ProductDto
                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    Price = product.Price.Amount,
                    Currency = product.Price.Currency,
                    Description = product.Description,
                    StockQuantity = product.StockQuantity,
                    IsActive = product.IsActive,
                    LowStockThreshold = product.LowStockThreshold,
                    IsLowStock = product.IsLowStock(),
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                return Result<ProductDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                return Result<ProductDto>.Failure($"Ocurrió un error al retornar el producto: {ex.Message}");
            }
        }
    }
}
