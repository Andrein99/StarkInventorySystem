using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Interfaces.Services
{
    /// <summary>
    /// Servicio para obtener información del usuario actual.
    /// La capa de aplicación define la interface, y la capa de infraestructura implementa la lógica.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// ID del usuario actual, o null si no está autenticado.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// Email del usuario actual, o null si no está autenticado.
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Nombre de usuario del usuario actual, o null si no está autenticado.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Indica si el usuario está autenticado.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Indica si el usuario actual pertenece al rol especificado.
        /// </summary>
        /// <param name="role">Rol de usuario</param>
        /// <returns>True o False, dependiendo de si pertenece al rol.</returns>
        bool IsInRole(string role);

        /// <summary>
        /// Indica si el usuario actual tiene el claim especificado.
        /// </summary>
        /// <param name="claimType">Tipo de claim</param>
        /// <param name="claimValue">Valor del claim</param>
        /// <returns>True o False, dependiendo de si tiene claims.</returns>
        bool HasClaim(string claimType, string claimValue);
    }
}
