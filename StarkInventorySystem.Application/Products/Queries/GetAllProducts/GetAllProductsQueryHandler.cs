using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.ProductDtos;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Queries.GetAllProducts
{
    /// <summary>
    /// Manejador para la consulta GetAllProductsQuery.
    /// </summary>
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PagedResult<ProductDto>>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsQueryHandler(
            IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<PagedResult<ProductDto>>> HandleAsync(GetAllProductsQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Obtener todos los productos
                var allProducts = await _productRepository.GetAllAsync(cancellationToken);

                // Aplicar filtro por estado de activación si se proporciona
                var query = allProducts.AsQueryable();

                if (request.IsActive.HasValue)
                {
                    query = query.Where(p => p.IsActive == request.IsActive.Value);
                }

                // Obtener la cantidad total de productos después del filtro
                var totalCount = query.Count();

                // Aplicar paginación
                var products = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                // Mapear a DTOs
                var productDtos = products.Select(product => new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Sku = product.Sku,
                    Price = product.Price.Amount,
                    Currency = product.Price.Currency,
                    Description = product.Description,
                    StockQuantity = product.StockQuantity,
                    LowStockThreshold = product.LowStockThreshold,
                    IsActive = product.IsActive,
                    IsLowStock = product.IsLowStock(),
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                }).ToList();

                // Crear resultado paginado
                var pagedResult = PagedResult<ProductDto>.Create(
                    productDtos,
                    totalCount,
                    request.PageNumber,
                    request.PageSize);

                return Result<PagedResult<ProductDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                return Result<PagedResult<ProductDto>>.Failure(
                    $"Un error ocurrió al retornar los productos: {ex.Message}");
            }
        }
    }
}
