using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.OrderDtos;
using StarkInventorySystem.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<OrderDto>> HandleAsync(
            GetOrderByIdQuery request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);

                if (order == null)
                {
                    return Result<OrderDto>.Failure($"El pedido con ID {request.OrderId} no fue encontrado.");
                }

                // Preparando elementos para mapear a DTO
                AddressDto shippingAddress = new AddressDto
                {
                    Street = order.ShippingAddress.Street,
                    City = order.ShippingAddress.City,
                    State = order.ShippingAddress.State,
                    PostalCode = order.ShippingAddress.PostalCode,
                    Country = order.ShippingAddress.Country
                };

                List<OrderItemDto> items = order.Items
                    .Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice.Amount,
                        Currency = i.UnitPrice.Currency,
                        Subtotal = i.Subtotal.Amount
                    }).ToList();

                // Mapear la entidad Order a OrderDto
                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    ShippingAddress = shippingAddress,
                    Status = order.Status,
                    Total = order.Total.Amount,
                    Currency = order.Total.Currency,
                    Items = items,
                    CreatedAt = order.CreatedAt,
                    ConfirmedAt = order.ConfirmedAt,
                    ShippedAt = order.ShippedAt,
                    DeliveredAt = order.DeliveredAt,
                    CancelledAt = order.CancelledAt,
                    CancellationReason = order.CancellationReason
                };

                return Result<OrderDto>.Success(orderDto);
            }
            catch (Exception ex)
            {
                return Result<OrderDto>.Failure($"Ocurrió un error al obtener la orden: {ex.Message}");
            }
        }
    }
}
