using StarkInventorySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        // Traen un solo elemento
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        
        // Traen múltiples elementos
        Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default);
        Task<List<Product>> GetActiveProductsAsync(CancellationToken cancellationToken);

        // Verificar que existen
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);


        // Añadir
        Task AddAsync(Product product, CancellationToken cancellationToken = default);

        // Actualizar
        void Update(Product product);

        // Eliminar
        void Delete(Product product);
    }
}
