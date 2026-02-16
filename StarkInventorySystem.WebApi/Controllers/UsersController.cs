using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StarkInventorySystem.Infrastructure.Identity.Entities;

namespace StarkInventorySystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Listar todos los usuarios con sus roles.
        /// Admin only.
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        /// <response code="200">Usuarios retornados correctamente</response>
        /// <response code="401">No autenticado</response>
        /// <response code="403">No autorizado (requiere Admin role)</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Obteniendo todos los usuarios...");

            var users = _userManager.Users.ToList();

            var userDtos = new List<object>();

            foreach (var user in users) 
            { 
                var roles = await _userManager.GetRolesAsync(user);

                userDtos.Add(new {
                    id = user.Id,
                    email = user.Email,
                    userName = user.UserName,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    roles = roles
                });
            }

            return Ok(new
            {
                count = userDtos.Count,
                users = userDtos
            });
        }



        /// <summary>
        /// Asigna un rol a un usuario existente.
        /// Admin solamente.
        /// </summary>
        /// <param name="request">Email del usuario y el rol a asignar</param>
        /// <returns>Confirmación exitosa</returns>
        /// <response code="200">Rol asignado correctamente</response>
        /// <response code="400">Rol inválido o usuario no encontrado</response>
        /// <response code="401">No autenticado</response>
        /// <response code="403">No autorizado (requiere rol de Admin)</response>
        [HttpPost("assign-role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            _logger.LogInformation("Asignar rol {Role} al usuario {Email}", request.Role, request.Email);

            // Encontrar usuario por email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogInformation("Usuario no encontrado: {Email}", request.Email);
                return BadRequest(new { error = $"El usuario con email '{request.Email}' no fue encontrado." });
            }

            // Roles válidos
            var validRoles = new[] { "Admin", "Customer", "InventoryManager", "OrderManager", "WarehouseStaff" };
            if (!validRoles.Contains(request.Role))
            {
                return BadRequest(new
                {
                    error = $"Rol inválido. Los roles válidos son: {string.Join(", ", validRoles)}"
                });
            }

            // Verificar si el usuario ya tiene el rol
            if (await _userManager.IsInRoleAsync(user, request.Role))
            {
                return Ok(new { message = $"El usuario '{user.Email}' ya tiene el role '{request.Role}'." });
            }

            // Asignar rol al usuario
            var result = await _userManager.AddToRoleAsync(user, request.Role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Error al asignar el rol {Role} al usuario {Email}: {Errors}", 
                    request.Role, request.Email, errors);
                return BadRequest(new { error = $"Error al asignar el rol: {errors}" });
            }

            _logger.LogInformation("Role {Role} asignado correctamente al usuario {Email}", request.Role, request.Email);

            return Ok(new
            {
                message = $"El rol '{request.Role}' ha sido asignado al usuario '{user.Email}'.",
                email = request.Email,
                role = request.Role
            });
        }

        /// <summary>
        /// Remover rol de usuario.
        /// Admin solamente.
        /// </summary>
        /// <param name="request">Email del usuario y el rol a remover</param>
        /// <returns>Confirmación exitosa</returns>
        /// <response code="200">Rol removido correctamente</response>
        /// <response code="400">No tiene el rol o usuario no encontrado</response>
        /// <response code="401">No autenticado</response>
        /// <response code="403">No autorizado (requiere rol de Admin)</response>
        [HttpPost("remove-role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleRequest request)
        {
            _logger.LogInformation("Removiendo el rol {Role} del usuario {Email}", request.Role, request.Email);

            // Encontrar usuario por email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { error = $"El usuario con email '{request.Email}' no fue encontrado." });
            }

            // Verificar si el usuario tiene el rol
            if (!await _userManager.IsInRoleAsync(user, request.Role))
            {
                return BadRequest(new
                {
                    error = $"El usuario '{request.Email}' no tiene el rol '{request.Role}'."
                });
            }

            // Remover rol del usuario
            var result = await _userManager.RemoveFromRoleAsync(user, request.Role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { error = $"Fallo al remover el rol: {errors}" });
            }

            _logger.LogInformation("Rol {Role} removido correctamete del usuario {Email}", request.Role, request.Email);

            return Ok(new
            {
                message = $"Role '{request.Role}' removido del usuario '{request.Email}' correctamente."
            });
        }

        /// <summary>
        /// Obtiene todos los roles asignados al usuario.
        /// Admin solamente.
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <returns>Lista de roles</returns>
        /// <response code="200">Roles retornados correctamente</response>
        /// <response code="400">Usuario no encontrado</response>
        /// <response code="401">No autenticado</response>
        /// <response code="403">No autorizado (require rol Admin)</response>
        [HttpGet("{email}/roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            _logger.LogInformation("Obteniendo roles para el usuario {Email}", email);

            // Encontrar usuario por email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { error = $"El usuario con email '{email}' no fue enconrtado." });
            }

            // Obtener roles
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                email = user.Email,
                roles = roles
            });
        }


        #region Request Models

        /// <summary>
        /// Modelo request para asignar/remover roles.
        /// </summary>
        public class AssignRoleRequest
        {
            /// <summary>
            /// La dirección de correo electrónico del usuario
            /// </summary>
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// Rol a asignar/remover
            /// Valores válidos: Admin, Customer, InventoryManager, OrderManager, WarehouseStaff
            /// </summary>
            public string Role { get; set; } = string.Empty;
        }

        #endregion
    }
}
