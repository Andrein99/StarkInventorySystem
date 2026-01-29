using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.UpdateProductPrice
{
    /// <summary>
    /// Validador para el comando UpdateProductPriceCommand.
    /// </summary>
    public class UpdateProductPriceValidator : AbstractValidator<UpdateProductPriceCommand>
    {
        public UpdateProductPriceValidator() 
        { 
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("El Id del producto no puede estar vacío.");
            RuleFor(x => x.NewPrice)
                .GreaterThan(0).WithMessage("El nuevo precio debe ser mayor que cero.")
                .LessThanOrEqualTo(1000000).WithMessage("El nuevo precio no puede exceder 1,000,000.");
            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("El tipo de moneda no puede estar vacía.")
                .Length(3).WithMessage("El tipo de moneda debe tener exactamente 3 caracteres en formato ISO.")
                .Matches("^[A-Z]{3}$").WithMessage("El tipo de moneda debe tener 3 letras en mayúscula");
        }
    }
}
