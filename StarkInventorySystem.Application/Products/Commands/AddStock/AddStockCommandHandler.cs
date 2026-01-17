using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Domain.DomainExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.AddStock
{
    class AddStockCommandHandler : IRequestHandler<AddStockCommand, Result>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddStockCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> HandleAsync(
            AddStockCommand request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Obtener el producto
                var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

                if (product == null)
                {
                    return Result.Failure($"El producto con ID {request.ProductId} no fue encontrado.");
                }

                // Agregar stock usando la lógica de negocio de la entidad
                product.AddStock(request.QuantityToAdd);

                // Persistir los cambios
                _productRepository.Update(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException ex)
            {
                // Fallo en la regla de negocio
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                // Error inesperado
                return Result.Failure($"Ocurrió un error al agregar stock al producto: {ex.Message}");
            }
        }
    }
}
