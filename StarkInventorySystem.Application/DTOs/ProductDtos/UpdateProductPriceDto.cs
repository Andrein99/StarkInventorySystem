using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Application.DTOs.ProductDtos
{
    /// <summary>
    /// DTO para actualizar el precio de un producto.
    /// </summary>
    public class UpdateProductPriceDto
    {
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }
}
