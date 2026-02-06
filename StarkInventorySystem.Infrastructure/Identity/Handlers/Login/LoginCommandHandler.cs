using Microsoft.AspNetCore.Identity;
using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.Identity;
using StarkInventorySystem.Infrastructure.Identity.Entities;
using StarkInventorySystem.Infrastructure.Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Identity.Handlers.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<AuthenticationResponse>> HandleAsync(LoginCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Encontrar usuario por email o username
                var user = await _userManager.FindByEmailAsync(request.EmailOrUsername);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(request.EmailOrUsername);
                }

                if (user == null)
                {
                    return Result<AuthenticationResponse>.Failure("Credenciales inválidas.");
                }

                // Verificar contraseña usando SignInManager (maneja lockout, 2FA, etc.)
                var result = await _signInManager.CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    lockoutOnFailure: true); // Permite la lockout protection

                if (!result.Succeeded)
                {
                    if (result.IsLockedOut)
                    {
                        return Result<AuthenticationResponse>.Failure(
                            "La cuenta está bloqueada por múltiples intentos fallidos. Intente más tarde.");
                    }

                    return Result<AuthenticationResponse>.Failure("Credenciales inválidas.");
                }

                // Verificar que el usuario esté activo.
                if (!user.IsActive)
                {
                    return Result<AuthenticationResponse>
                        .Failure("La cuenta está desactivada. Contacte al administrador.");
                }

                // Verificar si la dirección de correo electrónico está confirmada.
                if (!user.EmailConfirmed)
                {
                    return Result<AuthenticationResponse>
                        .Failure("Debe confirmar su correo electrónico antes de iniciar sesión.");
                }

                // Actualizar el último inicio de sesión
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Generar el token JWT
                var (token, expiration) = await _jwtTokenService.GenerateTokenAsync(user);
                
                // Obtener roles de usuario
                var roles = await _userManager.GetRolesAsync(user);

                var response = new AuthenticationResponse
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    Username = user.UserName ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList(),
                    Token = token,
                    TokenExpiration = expiration
                };

                return Result<AuthenticationResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<AuthenticationResponse>.Failure(
                    $"Error inesperado al iniciar sesión: {ex.Message}");
            }
        }
    }
}
