using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Identity.Handlers.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El formato del correo electrónico no es válido.")
                .MaximumLength(100);

            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50)
                .Matches("^[a-zA-Z0-9_-]+$")
                .WithMessage("El nombre de usuario solo puede contener letras, números, guiones y guiones bajos.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100)
                .Must(BeAValidPassword)
                .WithMessage("La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Las contraseñas no coinciden.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);
        }

        private bool BeAValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
    }
}
