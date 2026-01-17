using Microsoft.Extensions.DependencyInjection;
using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Mediator;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registrar nuestro Mediator personalizado.
            services.AddScoped<IMediator, Mediator>();

            // Registrar todos los handlers automáticamente de este assembly.
            services.AddHandlers(Assembly.GetExecutingAssembly());

            // Registrar validadores de FluentValidation desde el ensamblado actual.
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }

        private static IServiceCollection AddHandlers(
            this IServiceCollection services,
            Assembly assembly)
        {
            // Buscar todos los tipos en el assembly que implementen IRequestHandler<,>
            var handlerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                .ToList();

            // Registrar cada handler en el contenedor de servicios
            foreach (var handlerType in handlerTypes)
            {
                // Obtener la interface IRequestHandler<,> que esta clase implementa
                var handlerInterfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    .ToList();

                foreach (var handlerInterface in handlerInterfaces)
                {
                    // Registrar: IRequestHandler<CreateProductCommand, Result<Guid>> -> CreateProductCommandHandler
                    services.AddScoped(handlerInterface, handlerType);
                }
            }

            return services;
        }
    }
}
