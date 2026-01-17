using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Orders.Commands.CreateOrder
{
    /// <summary>
    /// Validador para el comando CreateOrderCommand.
    /// </summary>
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("El ID del cliente es obligatorio.");
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("La dirección de envío es obligatoria.")
                .ChildRules(address =>
                {
                    address.RuleFor(a => a.Street)
                        .NotEmpty().WithMessage("La calle es obligatoria.")
                        .MaximumLength(200).WithMessage("La calle no puede exceder los 200 caracteres.");
                    address.RuleFor(a => a.City)
                        .NotEmpty().WithMessage("La ciudad es obligatoria.")
                        .MaximumLength(100).WithMessage("La ciudad no puede exceder los 100 caracteres.");
                    address.RuleFor(a => a.State)
                        .NotEmpty().WithMessage("El estado/provincia es obligatorio.")
                        .MaximumLength(100).WithMessage("El estado/provincia no puede exceder los 100 caracteres.");
                    address.RuleFor(a => a.PostalCode)
                        .NotEmpty().WithMessage("El código postal es obligatorio.")
                        .MaximumLength(20).WithMessage("El código postal no puede exceder los 20 caracteres.");
                    address.RuleFor(a => a.Country)
                        .NotEmpty().WithMessage("El país es obligatorio.")
                        .MaximumLength(100).WithMessage("El país no puede exceder los 100 caracteres.");
                });
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("La orden debe contener al menos un ítem.")
                .Must(items => items != null && items.Count > 0)
                .WithMessage("La orden debe contener al menos un ítem.");
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty().WithMessage("El ID del producto es obligatorio.");
                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.")
                    .LessThanOrEqualTo(10000).WithMessage("La cantidad no debe exceder las 10,000 unidades");
            });
        }
    }
}
