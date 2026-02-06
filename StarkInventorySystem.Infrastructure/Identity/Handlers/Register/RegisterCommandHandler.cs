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

namespace StarkInventorySystem.Infrastructure.Identity.Handlers.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        async Task<Result<AuthenticationResponse>> IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>.HandleAsync(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar que el correo no exista en la BBDD.
                var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingUserByEmail != null)
                {
                    return Result<AuthenticationResponse>.Failure(
                        "El correo electrónico ya está registrado.");
                }

                // Verificar que el username no exista en la BBDD. 
                var existingUserByUsername = await _userManager.FindByNameAsync(request.Username);
                if (existingUserByUsername != null)
                {
                    return Result<AuthenticationResponse>.Failure(
                        "El nombre de usuario ya está en uso.");
                }

                // Crear nuevo usuario
                var user = new ApplicationUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    EmailConfirmed = true, // Autoconfirmado para demo
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                };

                // El UserManager maneja el hasheado automáticamente
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result<AuthenticationResponse>.Failure(
                        $"Error al crear usuario: {errors}");
                }

                // Asignar rol por defecto
                await _userManager.AddToRoleAsync(user, "Customer");

                // Actualizar último inicio de sesión
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Generar JWT token
                var (token, expiration) = await _jwtTokenService.GenerateTokenAsync(user);

                // Obtener roles del usuario
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
                    $"Error inesperado al registrar usuario: {ex.Message}");
            }
        }
    }
}
