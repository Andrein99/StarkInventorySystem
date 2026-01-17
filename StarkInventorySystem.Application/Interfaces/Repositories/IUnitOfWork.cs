using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Guarda los cambios realizados en el contexto de la base de datos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Inicia una transacción de base de datos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns></returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Hace commit a la transacción de base de datos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns></returns>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Hace rollback a la transacción de base de datos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns></returns>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
