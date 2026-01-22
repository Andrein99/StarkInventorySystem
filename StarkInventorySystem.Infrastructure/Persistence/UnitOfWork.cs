using Microsoft.EntityFrameworkCore.Storage;
using StarkInventorySystem.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("Una transacción ya está en progreso.");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No hay una transacción en progreso para hacer commit.");
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No hay una transacción en progreso para hacer rollback.");
            }

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // SaveChanges hace commit de todos los cambios realizados en la transacción
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
