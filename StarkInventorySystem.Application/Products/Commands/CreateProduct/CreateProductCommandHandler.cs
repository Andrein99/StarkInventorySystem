using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Domain.DomainExceptions;
using StarkInventorySystem.Domain.Entities;
using StarkInventorySystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> HandleAsync(
            CreateProductCommand request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Regla de negocio: El SKU debe ser único
                if (await _productRepository.SkuExistsAsync(request.Sku, cancellationToken))
                {
                    return Result<Guid>.Failure($"El producto con SKU '{request.Sku}' ya existe.");
                }

                // Crear el objeto de valor Money
                var price = Money.Create(request.Price, request.Currency);

                // Crear la entidad Product (La lógica de negocio aplica las validaciones necesarias)
                var product = Product.Create(
                    request.Name,
                    request.Sku,
                    price,
                    request.Description
                );

                // Asignar el umbral bajo de stock si se proporciona
                if (request.LowStockThreshold.HasValue)
                {
                    product.SetLowStockThreshold(request.LowStockThreshold.Value);
                }

                // Persistir en la base de datos
                await _productRepository.AddAsync(product, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Retornar success con el ID del producto creado
                return Result<Guid>.Success(product.Id);
            }
            catch (DomainException ex)
            {
                // Regla de negocio incumplida - Falla esperada
                return Result<Guid>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                // Error técnico inesperado
                // Loguear el error en producción
                return Result<Guid>.Failure($"Error inesperado al crear el producto: {ex.Message}");
            }
        }
    }
}
