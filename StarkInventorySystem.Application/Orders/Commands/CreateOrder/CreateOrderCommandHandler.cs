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

namespace StarkInventorySystem.Application.Orders.Commands.CreateOrder
{
    /// <summary>
    /// Manejador para el comando de creación de una orden.
    /// </summary>
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> HandleAsync(
            CreateOrderCommand request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validar que haya al menos un item
                if (request.Items == null || !request.Items.Any())
                {
                    return Result<Guid>.Failure("La orden debe tener al menos un item.");
                }

                // Crear el objeto de valor Address
                var shippingAddress = Address.Create(
                    request.ShippingAddress.Street,
                    request.ShippingAddress.City,
                    request.ShippingAddress.State,
                    request.ShippingAddress.PostalCode,
                    request.ShippingAddress.Country
                );

                // Crear la orden
                var order = Order.Create(request.CustomerId, shippingAddress);

                // Cargar todos los productos en una query (optimización)
                var productIds = request.Items.Select(i => i.ProductId).ToList();
                var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
                var productDict = products.ToDictionary(p => p.Id);

                // Agregar items a la orden
                foreach (var itemDto in request.Items)
                {
                    if (!productDict.TryGetValue(itemDto.ProductId, out var product))
                    {
                        return Result<Guid>.Failure($"El producto con ID {itemDto.ProductId} no existe.");
                    }

                    // La entidad de dominio se asegura de cumplir la regla de negocios.
                    order.AddItem(product, itemDto.Quantity);
                }

                // Persistencia
                await _orderRepository.AddAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(order.Id);
            }
            catch (DomainException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Ocurrió un error inesperado al crear la orden: {ex.Message}.");
            }
        }
    }
}
