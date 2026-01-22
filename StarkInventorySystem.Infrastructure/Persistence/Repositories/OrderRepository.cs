using Microsoft.EntityFrameworkCore;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Domain.Entities;
using StarkInventorySystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de órdenes en el sistema de inventario.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega una nueva orden al sistema.
        /// </summary>
        /// <param name="order">Orden a agregar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            // Los OrderItem se agregan automáticamente debido a la configuración de cascada en EF Core
            await _context.Orders.AddAsync(order, cancellationToken);
        }

        /// <summary>
        /// Elimina una orden del sistema.
        /// </summary>
        /// <param name="order">Orden a eliminar</param>
        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }

        /// <summary>
        /// Verifica si una orden existe por su Id.
        /// </summary>
        /// <param name="id">Id de la orden</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con booleano</returns>
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AnyAsync(o => o.Id == id, cancellationToken);
        }

        /// <summary>
        /// Obtiene todas las órdenes del sistema.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con lista de ordenes</returns>
        public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include("_items") // Incluir los OrderItems relacionados
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene las órdenes de un cliente específico.
        /// </summary>
        /// <param name="customerId">Id del cliente</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con lista de ordenes</returns>
        public async Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include("_items") // Incluir los OrderItems relacionados
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt) // Más recientes
                .ToListAsync(cancellationToken);
        }


        /// <summary>
        /// Obtiene una orden por su Id.
        /// </summary>
        /// <param name="id">Id de la orden</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con Orden</returns>
        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Carga la orden sin items (query más ligera)
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        /// <summary>
        /// Obtiene una orden por su Id incluyendo sus items.
        /// </summary>
        /// <param name="id">Id de la orden</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con Orden</returns>
        public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Carga la orden con sus items relacionados
            // Es importante para el procesamiento de la orden
            return await _context.Orders
                .Include("_items")
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        /// <summary>
        /// Obtiene todas las órdenes pendientes.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con lista de ordenes pendientes</returns>
        public async Task<List<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include("_items") // Incluir los OrderItems relacionados
                .Where(o => o.Status == OrderStatus.Pending)
                .OrderBy(o => o.CreatedAt) // Más antiguas primero
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Actualiza una orden existente.
        /// </summary>
        /// <param name="order">Orden a actualizar</param>
        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }
    }
}
