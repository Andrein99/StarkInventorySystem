using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Infrastructure.Identity.Handlers.Login;
using StarkInventorySystem.Infrastructure.Identity.Handlers.Register;

namespace StarkInventorySystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IMediator mediator,
            ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Registra a un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="command">Detalles de registro de usuario</param>
        /// <returns>Authentication response con token JWT</returns>
        /// <response code="200">Usuario registrado correctamente</response>
        /// <response code="400">Input inválido o el usuario ya existe.</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            _logger.LogInformation("Registrando nuevo usuario: {Email}", command.Email);

            var result = await _mediator.SendAsync(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("El registro falló para el correo {Email}: {Error}", command.Email, result.Error);
                return BadRequest(new { error = result.Error, errors = result.Errors });
            }

            _logger.LogInformation("El usuario fue registrado correctamente: {Email}", command.Email);

            return Ok(result.Value);
        }

        /// <summary>
        /// Autentica a un usuario en el sistema, y retorna un token JWT.
        /// </summary>
        /// <param name="command">Credenciales de inicio de sesión</param>
        /// <returns>Authentication response con token JWT</returns>
        /// <response code="200">Inicio de sesión exitoso</response>
        /// <response code="400">Credenciales inválidas o problemas con la cuenta</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            _logger.LogInformation("Intento de inicio de sesión: {EmailOrUsername}", command.EmailOrUsername);

            var result = await _mediator.SendAsync(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("El inicio de sesión falló para {EmailOrUsername}: {Error}", command.EmailOrUsername,
                    result.Error);
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("El usuario inició sesión correctamente: {UserId}", result.Value.UserId);

            return Ok(result.Value);
        }

        /// <summary>
        /// Obtiene el perfil del usuario actualmente autenticado.
        /// Requiere autenticación.
        /// </summary>
        /// <returns>Perfil de usuario actual</returns>
        /// <response code="200">Perfil de usuario retornado</response>
        /// <response code="401">No autenticado</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            // Get claims from JWT token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                userId = userId,
                email = email,
                username = username,
                roles = roles
            });
        }

        /// <summary>
        /// Endpoint de prueba para verificar que la autenticación está funcionando.
        /// Requires authentication.
        /// </summary>
        /// <returns>Success message</returns>
        /// <response code="200">Authentication successful</response>
        /// <response code="401">Not authenticated</response>
        [HttpGet("test-auth")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult TestAuth()
        {
            return Ok(new { message = "You are authenticated!", user = User.Identity?.Name });
        }

        /// <summary>
        /// Endpoint de prueba que quiere rol de admin.
        /// </summary>
        /// <returns>Success message</returns>
        /// <response code="200">User is admin</response>
        /// <response code="401">Not authenticated</response>
        /// <response code="403">Not authorized (not an admin)</response>
        [HttpGet("test-admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult TestAdmin()
        {
            return Ok(new { message = "You are an admin!", user = User.Identity?.Name });
        }
    }
}
