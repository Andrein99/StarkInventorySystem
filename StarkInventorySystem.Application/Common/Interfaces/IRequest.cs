using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Common.Interfaces
{
    /// <summary>
    /// Marca de interfaz para requests que retornan una respuesta de tipo TResponse.
    /// Similar a MediatR IRequest<TResponse>.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequest<out TResponse>
    {
        // Marca de interfaz para requests que retornan una respuesta de tipo TResponse.
    }

    /// <summary>
    /// Manejador de requests genérico.
    /// Similar a MediatR IRequestHandler.
    /// </summary>
    /// <typeparam name="TRequest">El tipo de request a manejar</typeparam>
    /// <typeparam name="TResponse">El tipo de response a retornar</typeparam>
    public interface IRequestHandler<in TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Maneja la request de forma asíncrona.
        /// </summary>
        /// <param name="request">La request a manejar</param>
        /// <param name="cancellationToken">El cancellationToken</param>
        /// <returns>La respuesta</returns>
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
