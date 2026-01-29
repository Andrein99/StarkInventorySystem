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

namespace StarkInventorySystem.Application.Products.Commands.UpdateProductInfo
{
    /// <summary>
    /// Manejador para el comando UpdateProductInfoCommand.
    /// </summary>
    public class UpdateProductInfoCommandHandler : IRequestHandler<UpdateProductInfoCommand, Result>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductInfoCommandHandler(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> HandleAsync(UpdateProductInfoCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Cargar producto a actualizar
                var product = await _productRepository.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    return Result.Failure($"Producto con ID {request.ProductId} no encontrado.");
                }

                
                // Actualizar el nombre y descripción del producto
                product.UpdateInfo(request.NewName, request.NewDescription);

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
                return Result.Failure($"Error inesperado al actualizar la información del producto: {ex.Message}");
            }
        }
    }
}
