using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Identity.Handlers.Login
{
    /// <summary>
    /// Validador para el LoginCommand
    /// La validación del login es mínima intencionalmente para evitar darle información a atacantes (Information Disclosure)
    /// </summary>
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            // Don't validate email format - could be username
            // Don't validate password complexity - already set during registration
            // Only check for empty fields

            RuleFor(x => x.EmailOrUsername)
                .NotEmpty()
                .WithMessage("El correo electrónico o nombre de usuario es obligatorio.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("La contraseña es obligatoria.");
        }
    }
}
