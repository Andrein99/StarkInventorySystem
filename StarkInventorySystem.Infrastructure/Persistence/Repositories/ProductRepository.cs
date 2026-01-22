using Microsoft.EntityFrameworkCore;
using StarkInventorySystem.Application.Interfaces.Repositories;
using StarkInventorySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación del repositorio de productos utilizando Entity Framework Core.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Añade un nuevo producto al contexto de la base de datos.
        /// </summary>
        /// <param name="product">Producto a añadir</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product, cancellationToken);
        }

        /// <summary>
        /// Elimina un producto del contexto de la base de datos.
        /// </summary>
        /// <param name="product">Producto a borrar</param>
        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        /// <summary>
        /// Verifica si un producto existe por su Id.
        /// </summary>
        /// <param name="id">Id del producto</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con booleano</returns>
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(p => p.Id == id, cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los productos activos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con lista de productos</returns>
        public async Task<List<Product>> GetActiveProductsAsync(CancellationToken cancellationToken)
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los productos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con lista de productos</returns>
        public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un producto por su Id.
        /// </summary>
        /// <param name="id">Id del producto</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con Producto si existe</returns>
        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        /// <summary>
        /// Obtiene múltiples productos por sus Ids.
        /// </summary>
        /// <param name="ids">Ids de los productos</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con lista de productos</returns>
        public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un producto por su SKU.
        /// </summary>
        /// <param name="sku">SKU del producto</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con el Producto si existe</returns>
        public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken);
        }


        /// <summary>
        /// Obtiene los productos con bajo stock.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con lista de productos</returns>
        public async Task<List<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.StockQuantity <= p.LowStockThreshold) // Traer los productos con un stock inferior al umbral de stock del producto
                .Where(p => p.IsActive) // Solo productos activos
                .OrderBy(p => p.StockQuantity) // Ordenar por cantidad de stock ascendente, para tener los que menos unidades tengan primero
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Verifica si un producto existe por su SKU.
        /// </summary>
        /// <param name="sku">SKU del producto</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task con booleano</returns>
        public async Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(p => p.Sku == sku, cancellationToken);
        }

        /// <summary>
        /// Actualiza un producto en el contexto de la base de datos.
        /// </summary>
        /// <param name="product">Producto a actualizar</param>
        public void Update(Product product)
        {
            // EF Core le hace seguimiento automáticamente a las entidades recuperadas
            // Esto explícitamente marca la entidad como modificada (aunque no es estrictamente necesario)
            _context.Products.Update(product);
        }
    }
}
