using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Common.Interfaces
{
    /// <summary>
    /// Interfaz de mediador para enviar requests a sus handlers correspondientes.
    /// 
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Envía una request a su handler correspondiente y retorna la respuesta.
        /// </summary>
        /// <typeparam name="TResponse">El tipo de response esperada</typeparam>
        /// <param name="request">La request a enviar</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>La response del manejador</returns>
        Task<TResponse> SendAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default);
    }
}
