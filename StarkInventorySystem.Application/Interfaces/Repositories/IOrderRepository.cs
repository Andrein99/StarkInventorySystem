using StarkInventorySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        // Traen un solo elemento
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);

        // Traen múltiples elementos
        Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<List<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
        Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);

        // Verificar que existen
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

        // Añadir
        Task AddAsync(Order order, CancellationToken cancellationToken = default);

        // Actualizar
        void Update(Order order);

        // Eliminar
        void Delete(Order order);
    }
}
