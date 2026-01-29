using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.Products.Commands.UpdateProductInfo
{
    /// <summary>
    /// Comando para actualizar la información de un producto (nombre y descripción).
    /// </summary>
    public record UpdateProductInfoCommand : IRequest<Result>
    {
        public Guid ProductId { get; init; }
        public string NewName { get; init; }
        public string NewDescription { get; init; }
    }
}
