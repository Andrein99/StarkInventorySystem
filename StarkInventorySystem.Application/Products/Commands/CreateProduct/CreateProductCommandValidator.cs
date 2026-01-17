using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.CreateProduct
{
    /// <summary>
    /// Validador para el comando CreateProductCommand.
    /// </summary>
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre del producto no puede exceder los 200 caracteres.");
            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("El SKU del producto es obligatorio.")
                .MaximumLength(50).WithMessage("El SKU del producto no puede exceder los 50 caracteres.")
                .Matches("^[A-Z0-9-]+$").WithMessage("El SKU del producto solo puede contener letras mayúsculas, números y guiones.");
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("La descripción del producto no puede exceder los 1000 caracteres.");
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio del producto debe ser mayor que cero.");
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("El tipo de moneda es obligatoria.")
                .Length(3).WithMessage("El tipo de moneda debe ser una cadena de 3 caracteres (código ISO 4217, e.g. USD, EUR, COP).")
                .Matches("[A-Z]{3}$").WithMessage("El tipo de moneda debe ser 3 letras mayúsculas");
            RuleFor(x => x.LowStockThreshold)
                .GreaterThanOrEqualTo(0).When(x => x.LowStockThreshold.HasValue)
                .WithMessage("El umbral de bajo stock no puede ser negativo.");
        }
    }
}
