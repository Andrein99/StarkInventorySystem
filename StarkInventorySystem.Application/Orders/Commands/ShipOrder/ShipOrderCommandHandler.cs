using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Domain.DomainExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Commands.ShipOrder
{
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ShipOrderCommandHandler(IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> HandleAsync(
            ShipOrderCommand request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Cargar la orden
                var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

                if (order == null)
                {
                    return Result.Failure($"La orden con Id {request.OrderId} no fue encontrada.");
                }

                // La entidad Order maneja la lógica de negocio y la validación
                order.Ship();

                // Actualizar la orden
                _orderRepository.Update(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException dex)
            {
                // Error esperado en la lógica de negocio
                return Result.Failure(dex.Message);
            }
            catch (Exception ex)
            {
                // Error inesperado
                return Result.Failure($"Un error ocurrió al enviar la orden: {ex.Message}");
            }
        }
    }
}
