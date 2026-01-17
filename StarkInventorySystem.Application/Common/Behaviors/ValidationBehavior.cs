using FluentValidation;
using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(
            IRequestHandler<TRequest, TResponse> inner,
            IEnumerable<IValidator<TRequest>> validators)
        {
            _inner = inner;
            _validators = validators;
        }

        public async Task<TResponse> HandleAsync(
            TRequest request, 
            CancellationToken cancellationToken = default)
        {
            // Si no hay validadores, saltar la validación
            if (!_validators.Any())
            {
                return await _inner.HandleAsync(request, cancellationToken);
            }

            // Correr todos los validadores
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Recolectar fallos
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(failure => failure != null)
                .ToList();

            // Si hay fallos, devolver el resultado del error
            if (failures.Any())
            {
                var errors = failures.Select(failure => failure.ErrorMessage).ToList();

                // Intentar crear un fallo de Result
                // Esto asume que TResponse es Result o Result<T>
                if (typeof(TResponse).IsGenericType &&
                    typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    // Tipo Result<T>
                    var resultType = typeof(TResponse);
                    var failureMethod = resultType
                        .GetMethod(nameof(Result<object>.Failure),
                        new[] { typeof(List<string>) });

                    if (failureMethod != null) 
                    {
                        return failureMethod.Invoke(null, new object[] { errors }) as TResponse;
                    }
                }
                else if (typeof(TResponse) == typeof(Result))
                {
                    // Tipo Result
                    return Result.Failure(errors) as TResponse;
                }

                // Fallback: lanzar excepción si no se puede crear Result
                throw new ValidationException(failures);
            }

            // La validación pasó, continuar al manejador
            return await _inner.HandleAsync(request, cancellationToken);
        }
    }
}
