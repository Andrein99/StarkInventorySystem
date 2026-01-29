using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Domain.DomainExceptions;
using StarkInventorySystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.UpdateProductPrice
{
    /// <summary>
    /// Manejador para el comando UpdateProductPriceCommand.
    /// </summary>
    public class UpdateProductPriceCommandHandler : IRequestHandler<UpdateProductPriceCommand, Result>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductPriceCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> HandleAsync(UpdateProductPriceCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Cargar producto a actualizar
                var product = await _productRepository.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    return Result.Failure($"Producto con ID {request.ProductId} no encontrado.");
                }

                // Crear objeto de valor Money para el precio nuevo
                var newPrice = Money.Create(request.NewPrice, request.Currency);

                // Actualizar el precio del producto
                product.UpdatePrice(newPrice);

                // Actualizar en el repositorio
                _productRepository.Update(product);

                // Guardar cambios
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException dex)
            {
                // Manejar errores de dominio y retornar fallo con mensaje
                return Result.Failure(dex.Message);
            }
            catch (Exception ex)
            {
                // Manejar errores inesperados
                return Result.Failure($"Error inesperado al actualizar el precio del producto: {ex.Message}");
            }
        }
    }
}
