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

namespace StarkInventorySystem.Application.Orders.Commands.ConfirmOrder
{
    /// <summary>
    /// Manejador para el comando de confirmación de pedido.
    /// </summary>
    public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmOrderCommandHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task <Result> HandleAsync(
            ConfirmOrderCommand request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);

                if (order == null)
                {
                    return Result.Failure($"La orden con ID {request.OrderId} no fue encontrada.");
                }

                // IMPORTANTE: Reservar el stock antes de confirmar
                // Cargar todos los productos para esta orden
                var productIds = order.Items.Select(i => i.ProductId).ToList();
                var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
                var productDict = products.ToDictionary(p => p.Id);

                // Verificar y reservar stock
                foreach (var item in order.Items)
                {
                    if (productDict.TryGetValue(item.ProductId, out var product))
                    {
                        return Result.Failure($"El producto con ID {item.ProductId} ya no existe.");
                    }

                    // Verificar si hay stock suficiente
                    if (!product.HasSufficientStock(item.Quantity))
                    {
                        return Result.Failure($"No hay stock suficiente para el producto {product.Name}. Disponible: {product.StockQuantity}, Requerido: {item.Quantity}.");
                    }

                    // Reservar el stock (quitar del inventario disponible)
                    product.RemoveStock(item.Quantity);
                    _productRepository.Update(product);
                }

                // Confirmar la orden (cambiar estado)
                order.Confirm();
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
                return Result.Failure($"Ocurrió un error inesperado al crear la orden: {ex.Message}.");
            }
        }
    }
}
