using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Identity.Entities
{
    /// <summary>
    /// Entidad de usuario personalizada que extiende IdentityUser con propiedades adicionales.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del usuario
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la cuenta del usuario está activa.
        /// Soft delete - Usuarios desactivados no puedes iniciar sesión.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Hora y fecha cuando se creó el usuario
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Hora y fecha de la última conexión.
        /// Usado para analítica y auditing de seguridad.
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Obtiene el nombre completo del usuario.
        /// </summary>
        public string GetFullName() => $"{FirstName} {LastName}".Trim();

        // Note: The following properties are inherited from IdentityUser<Guid>:
        // - Id (Guid)
        // - UserName (string)
        // - NormalizedUserName (string)
        // - Email (string)
        // - NormalizedEmail (string)
        // - EmailConfirmed (bool)
        // - PasswordHash (string)
        // - SecurityStamp (string)
        // - ConcurrencyStamp (string)
        // - PhoneNumber (string)
        // - PhoneNumberConfirmed (bool)
        // - TwoFactorEnabled (bool)
        // - LockoutEnd (DateTimeOffset?)
        // - LockoutEnabled (bool)
        // - AccessFailedCount (int)
    }
}
