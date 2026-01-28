using Microsoft.AspNetCore.Mvc;
using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Products.Commands.AddStock;
using StarkInventorySystem.Application.Products.Commands.CreateProduct;
using StarkInventorySystem.Application.Products.Queries.GetLowStockProducts;
using StarkInventorySystem.Application.Products.Queries.GetProductById;

namespace StarkInventorySystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Crea un nuevo producto en el inventario.
        /// </summary>
        /// <param name="command">Detalles de la creación del producto</param>
        /// <returns>El ID del producto creado</returns>
        /// <response code="200">Producto creado exitosamente</response>
        /// <response code="400">Input inválido o ruptura de regla de negocio</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            _logger.LogInformation("Creando producto con SKU: {Sku}", command.Sku);

            var result = await _mediator.SendAsync(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Falló la creacón del producto: {Error}", result.Error);
                return BadRequest(new { error = result.Error, errors = result.Errors });
            }

            _logger.LogInformation("Producto creado exitosamente con ID: {ProductId}", result.Value);
            return Ok(new { id = result.Value, message = "Producto creado exitosamente"});
        }

        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        /// <param name="id">Id del producto</param>
        /// <returns>Detalles del producto</returns>
        /// <response code="200">Producto encontrado</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            _logger.LogInformation("Obteniendo producto con ID: {ProductId}", id);

            var query = new GetProductByIdQuery(id);
            var result = await _mediator.SendAsync(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("El producto no fue encontrado: {ProductId}", id);
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Obtiene la lista de productos con bajo stock.
        /// </summary>
        /// <returns>Lista de productos con stock bajo</returns>
        /// <response>Productos con bajo stock retornados correctamente</response>
        [HttpGet("low-stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLowStockProducts()
        {
            _logger.LogInformation("Obteniendo productos con bajo stock");

            var query = new GetLowStockProductsQuery();
            var result = await _mediator.SendAsync(query);

            if (result.IsFailure)
            {
                return BadRequest(new { error = result.Error });
            }

            return Ok(new
            {
                count = result.Value.Count,
                products= result.Value
            });
        }

        /// <summary>
        /// Agrega stock a un producto existente.
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="request">Cantidad de stock a añadir</param>
        /// <returns>Confirmación exitosa</returns>
        /// <response code="204">Stock añadido correctamente</response>
        /// <response code="400">Cantidad inválida o producto no encontrado</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpPost("{id}/stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddStock(Guid id, [FromBody] AddStockRequest request)
        {
            _logger.LogInformation("Agregando {Quantity} de stock para el producto {ProductId}", request.Quantity, id);

            var command = new AddStockCommand(id, request.Quantity);
            var result = await _mediator.SendAsync(command);

            if (result.IsFailure) 
            { 
                _logger.LogWarning("Falló al agregar stock para el producto {ProductId}: {Error}", id, result.Error);

                if (result.Error.Contains("no encontrado"))
                {
                    return NotFound(new { error = result.Error });
                }

                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Stock agregado exitosamente para el producto {ProductId}", id);
            return NoContent();
        }

        /// <summary>
        /// Actualiza el precio de un producto existente.
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="request">Detalles del nuevo precio</param>
        /// <returns>Confirmación exitosa</returns>
        /// <response code="204">El precio se actualizó correctamente</response>
        /// <response code="400">Precio inválido</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpPut("{id}/price")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] UpdatePriceRequest request)
        {
            _logger.LogInformation("Actualizando precio para el producto {ProductId} a {Price} {Currency}", id, request.Price, request.Currency);

            // TODO: Implementar UpdatePriceCommand y su handler
            return StatusCode(StatusCodes.Status501NotImplemented,
                new { message = "La funcionalidad de actualización de precios está lista pronto." });
        }
    }
}

#region Request Models

/// <summary>
/// Modelo de request para agregar stock a un producto.
/// </summary>
public class AddStockRequest
{
    public int Quantity { get; set; }
}

/// <summary>
/// Modelo de request para actualizar el precio de un producto.
/// </summary>
public class UpdatePriceRequest
{
    public decimal Price { get; set; }
    public string Currency { get; set; }
}

#endregion