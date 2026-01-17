using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Domain.DomainExceptions;
using StarkInventorySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Commands.CancelOrder
{
    /// <summary>
    /// Manejador para el comando de cancelar una orden.
    /// </summary>
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderCommandHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> HandleAsync(
            CancelOrderCommand request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Cargar la orden con items
                var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);

                if (order == null)
                {
                    return Result.Failure($"La orden con ID {request.OrderId} no fue encontrada.");
                }

                // Si la orden está confirmada, necesitamos liberar el stock reservado
                bool needToReleaseStock = order.Status == OrderStatus.Confirmed;

                if (needToReleaseStock)
                {
                    // Cargar todos los productos
                    var productIds = order.Items.Select(i => i.ProductId).ToList();
                    var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
                    var productDict = products.ToDictionary(p => p.Id);

                    // Liberar el que se había reservado para cada producto
                    foreach (var item in order.Items)
                    {
                        if (productDict.TryGetValue(item.ProductId, out var product))
                        {
                            // Añadir el stock de vuelta
                            product.AddStock(item.Quantity);
                            _productRepository.Update(product);
                        }
                    }
                }

                // Cancelar la orden (El dominio valida si la cancelación es permitida)
                order.Cancel(request.Reason);
                _orderRepository.Update(order);

                // Guardar cambios en una transacción
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Un error ocurrió mientras se cancelaba la orden: {ex.Message}");
            }
        }
    }
}
