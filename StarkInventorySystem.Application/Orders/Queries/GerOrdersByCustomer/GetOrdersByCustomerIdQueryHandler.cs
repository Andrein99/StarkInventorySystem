using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.OrderDtos;
using StarkInventorySystem.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Queries.GerOrdersByCustomer
{
    public class GetOrdersByCustomerIdQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, Result<List<OrderDto>>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByCustomerIdQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<Result<List<OrderDto>>> HandleAsync(GetOrdersByCustomerQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Cargar ordenes
                var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

                // Mapear a DTOs
                var orderDtos = orders.Select(orders => new OrderDto
                {
                    Id = orders.Id,
                    CustomerId = orders.CustomerId,
                    ShippingAddress = new AddressDto
                    {
                        Street = orders.ShippingAddress.Street,
                        City = orders.ShippingAddress.City,
                        State = orders.ShippingAddress.State,
                        PostalCode = orders.ShippingAddress.PostalCode,
                        Country = orders.ShippingAddress.Country
                    },
                    Status = orders.Status,
                    Total = orders.Total.Amount,
                    Currency = orders.Total.Currency,
                    Items = orders.Items.Select(item => new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        ProductSku = item.ProductSku,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice.Amount,
                        Currency = item.UnitPrice.Currency,
                        Subtotal = item.Subtotal.Amount
                    }).ToList(),
                    CreatedAt = orders.CreatedAt,
                    ConfirmedAt = orders.ConfirmedAt,
                    ShippedAt = orders.ShippedAt,
                    DeliveredAt = orders.DeliveredAt,
                    CancelledAt = orders.CancelledAt,
                    CancellationReason = orders.CancellationReason
                }).ToList();

                return Result<List<OrderDto>>.Success(orderDtos);
            }
            catch (Exception ex)
            {
                return Result<List<OrderDto>>.Failure($"Un error ocurrió retornando las órdenes del cliente {request.CustomerId}: {ex.Message}");
            }
        }
    }
}
