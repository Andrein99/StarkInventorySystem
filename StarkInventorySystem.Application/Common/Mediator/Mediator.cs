using StarkInventorySystem.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Common.Mediator
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TResponse> SendAsync<TResponse>(
            IRequest<TResponse> request, 
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Obtener los tipos de la response y la request
            var requestType = request.GetType();
            var responseType = typeof(TResponse);

            // Construir el tipo de la interfaz del handler: IRequestHandler<TRequest, TResponse>
            var handlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(requestType, responseType);

            // Resolver el handler desde el proveedor de servicios
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                throw new InvalidOperationException(
                    $"No hay handler registrado para el tipo de request {requestType.Name}. " +
                    $"Handler esperado: {handlerType.Name}");
            }

            // Invocar el método HandleAsync del handler
            var method = handlerType.GetMethod(
                nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync),
                new[] { requestType, typeof(CancellationToken) });

            if (method == null)
            {
                method = handlerType.GetMethod(
                    nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync),
                    new[] { requestType });
                
                if (method == null)
                {
                    throw new InvalidOperationException(
                        $"El método HandleAsync no fue encontrado en el handler {handlerType.Name}. " +
                        $"Asegurarse de que el handler implemente IRequestHandler<{requestType.Name}, {responseType.Name}>");
                }
                
            }

            // Invocar el handler con los parámetros correctos
            object[] parameters = method.GetParameters().Length == 2
                ? new object[] { request, cancellationToken }
                : new object[] { request };

            // Invocar el handler
            var result = method.Invoke(handler, parameters);

            // El resultado es un Task<TResponse>, por lo que hacemos un await.
            if (result is Task<TResponse> task)
            {
                return await task;
            }

            throw new InvalidOperationException(
                $"Handler {handlerType.Name} no retornó Task<{responseType.Name}>");
        }
    }
}
