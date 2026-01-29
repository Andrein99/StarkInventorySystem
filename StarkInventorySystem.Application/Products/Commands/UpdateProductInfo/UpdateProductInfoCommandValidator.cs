using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.UpdateProductInfo
{
    /// <summary>
    /// Validador para el comando UpdateProductInfoCommand.
    /// </summary>
    public class UpdateProductInfoCommandValidator : AbstractValidator<UpdateProductInfoCommand>
    {
        public UpdateProductInfoCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("El ID del producto no puede estar vacío.");
            RuleFor(x => x.NewName)
                .NotEmpty().WithMessage("El nuevo nombre del producto no puede estar vacío.")
                .MaximumLength(200).WithMessage("El nuevo nombre del producto no puede exceder los 200 caracteres.");
            RuleFor(x => x.NewDescription)
                .MaximumLength(1000).WithMessage("La nueva descripción del producto no puede exceder los 1000 caracteres.");
        }
    }
}
