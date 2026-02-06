using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StarkInventorySystem.Application.Interfaces.Services;
using StarkInventorySystem.Domain.Entities;
using StarkInventorySystem.Infrastructure.Identity.Configuration;
using StarkInventorySystem.Infrastructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Identity.Services
{
    /// <summary>
    /// Servicio para generar y validar JWT
    /// </summary>
    public interface IJwtTokenService
    {
        Task<(string Token, DateTime Expiration)> GenerateTokenAsync(ApplicationUser user);
    }
    
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtTokenService(
            IOptions<JwtSettings> jwtSettings,
            UserManager<ApplicationUser> userManager)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;

            if (!_jwtSettings.IsValid())
            {
                throw new InvalidOperationException(
                    "JWT settings no están configurados correctamente." + 
                    "Revisar appsettings.json en la sección [JwtSettings]." + 
                    "La SecretKey debe tener al menos 32 caracteres.");
            }

            _tokenHandler = new JwtSecurityTokenHandler();
            
        }

        /// <summary>
        /// Genera un token JWT para un usuario específico.
        /// El token contiene el ID del usuario, email, username, nombre y roles como claims.
        /// </summary>
        /// <param name="user">El usuario al que se le va a crear el token</param>
        /// <returns>Tupla de (JWT token string, datime de la expiración del token)</returns>
        public async Task<(string Token, DateTime Expiration)> GenerateTokenAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Calcular expiración
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            // Obtener roles de usuario de Identity
            var roles = await _userManager.GetRolesAsync(user);

            // Build Claims (Información embebida en el token)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Subject (ID del usuario)
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (unique token ID)

                // Claims personalizados
                new Claim("fullName", user.GetFullName()),
            };

            // Añadir los claims de roles (ASP.NET Core busca por ClaimTypes.Role)
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Crear la llave desde el secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // Crear token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = credentials
            };

            // Generar token
            var token = _tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = _tokenHandler.WriteToken(token);

            return (tokenString, expiration);
        }

        /// <summary>
        /// Obtiene el ID del usuario de un token JWT SIN una validación completa.
        /// Usar solamente cuando se necesite el ID del usuario sin que importe su expiración.
        /// ATENCIÓN: No verifica la firma o la expiración.
        /// </summary>
        /// <param name="token">JWT Token String</param>
        /// <returns>ID del usuario si está presente en el token, null en otro caso</returns>
        public Guid? GetUserIdFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                // Leer el token sin validación (sólo parsear la estructura JWT)
                var jwtToken = _tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }

                return null;
            }
            catch
            {
                // Token malformado
                return null;
            }
        }

        /// <summary>
        /// Valida un token JWT y retorna el ID del usuario si es válido.
        /// Chequea la firma, la expiración, el issuer, y la audience.
        /// </summary>
        /// <param name="token">JWT token string para validar</param>
        /// <returns>El ID del usuario si es válido, nulo si es inválido o está expirado</returns>
        public Guid? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            try
            {
                // Validar token con parámetros estrictos
                _tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true, // Check expiration
                    ClockSkew = TimeSpan.Zero // No tolerance for clock differences
                }, out SecurityToken validatedToken);

                // Extraer el ID del usuario de los claims
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }

                return null;
            }
            catch
            {
                // La validación del token falló (expiró, firma inválida, etc.)
                // No exponer detalles del error por seguridad
                return null;
            }
        }
    }
}
