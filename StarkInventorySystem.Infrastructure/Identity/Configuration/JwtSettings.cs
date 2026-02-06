using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Identity.Configuration
{
    /// <summary>
    /// Configuración para la generación y validación de JWT (JSON Web Token).
    /// </summary>
    public class JwtSettings
    {
        // Nombre de sección de configuración en appsettings.json
        public const string SectionName = "JwtSettings";

        /// <summary>
        /// Llave secreta para firmar los tokens JWT.
        /// Debe tener al menos 32 caracteres para el algoritmo HS256.
        /// EN PRODUCCIÓN: Guardar en Azure Key Vault, AWS Secrets Manager, or en variables de entorno
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Token issuer - Tipicamente la URL de la API or el nombre de la aplicación.
        /// Es usado para validar que el token vino de nuestro sistema.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Token audience - Tipicamente la aplicación cliente o los consumidores de la API.
        /// Es usado para validar que el objetivo del token es nuestra API.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Tiempo de expiración del token en minutos.
        /// Recomendación (Quizás TODO para después): 15 a 60 min por seguridad, con refresh token para sesiones más largas.
        /// </summary>
        public int ExpirationMinutes { get; set; }

        /// <summary>
        /// Valida que todas las configuraciones necesarias está configuradas correctamente.
        /// Llamado al iniciar para que falle rápidamente si está mal configurado.
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(SecretKey) &&
                SecretKey.Length >= 32 && // HS256 requiere mínimo 256 bits (32 bytes)
                !string.IsNullOrWhiteSpace(Issuer) &&
                !string.IsNullOrWhiteSpace(Audience) &&
                ExpirationMinutes > 0;
        }
    }
}
