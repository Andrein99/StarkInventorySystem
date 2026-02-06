using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Identity.Handlers.Login
{
    /// <summary>
    /// Comando para autenticar a un usuario y generar un token JWT.
    /// Soporta inicio de sesión ya sea con email como con username para mejorar la UX.
    /// </summary>
    public record LoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        /// <summary>
        /// Email del usuario o nombre de usuario
        /// </summary>
        public string EmailOrUsername { get; init; }

        /// <summary>
        /// La contraseña del usuario en texto plano (será hasheada para verificación)
        /// </summary>
        public string Password { get; init; }
    }
}
