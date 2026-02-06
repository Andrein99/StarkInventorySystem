using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using StarkInventorySystem.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Identity.Handlers.Register
{
    /// <summary>
    /// Comando para registar un nuevo usuario en el sistema
    /// </summary>
    public record RegisterCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string Email { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;

    }
}
