using Microsoft.AspNetCore.Mvc;
using StarkInventorySystem.Application.Common.Interfaces;
using StarkInventorySystem.Application.Orders.Commands.CancelOrder;
using StarkInventorySystem.Application.Orders.Commands.ConfirmOrder;
using StarkInventorySystem.Application.Orders.Commands.CreateOrder;
using StarkInventorySystem.Application.Orders.Queries.GetOrderById;

namespace StarkInventorySystem.WebApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar órdenes.
    /// Maneja toda las operaciones relacionadas con las órdenes como creación, consulta, confirmación, envío y entrega.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IMediator mediator,
            ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva orden.
        /// </summary>
        /// <param name="command">Detalles de la creación de la orden</param>
        /// <returns>El ID de la orden creada</returns>
        /// <response code="201">Orden creada satisfactoriamente</response>
        /// <response code="400">Input inválido o stock insuficiente</response>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            _logger.LogWarning("Creando una nueva orden para el cliente {CustomerId}", command.CustomerId);

            var result = await _mediator.SendAsync(command);

            if (result.IsFailure)
            {
                _logger.LogError("Error al crear la orden: {Error}", result.Error);
                return BadRequest(new { error = result.Error, errors = result.Errors });
            }

            _logger.LogInformation("Orden creada satisfactoriamente con ID: {OrderId}", result.Value);

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = result.Value },
                new { orderId = result.Value, message = "Orden creada satisfactoriamente" }
            );
        }

        /// <summary>
        /// Obtiene los detalles de una orden por su ID.
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <returns>Detalles de la orden con los items</returns>
        /// <response code="200">Orden encontrada</response>
        /// <response code="404">Order no encontrada</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            _logger.LogInformation("Obteniendo detalles de la orden con ID: {OrderId}", id);

            var query = new GetOrderByIdQuery(id);
            var result = await _mediator.SendAsync(query);

            if (result.IsFailure)
            {
                _logger.LogWarning("Orden no encontrada: {OrderId}", id);
                return NotFound(new { error = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderRequest request)
        {
            _logger.LogInformation("Cancelando la orden con ID: {OrderId} por la razón: {Reason}", id, request.Reason);

            var command = new CancelOrderCommand(id, request.Reason);
            var result = await _mediator.SendAsync(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Error al cancelar la orden {OrderId}: {Error}", id, result.Error);

                if (result.Error.Contains("not found"))
                {
                    return NotFound(new { error = result.Error });
                }

                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Orden {OrderId} cancelada satisfactoriamente", id);
            return NoContent();
        }


        /// <summary>
        /// Confirma la orden con el ID especificado.
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <returns>Confirmación exitosa</returns>
        /// <response code="204">Orden confirmada satisfactoriamente</response>
        /// <response code="400">No se puede confirmar la orden (estado incorrecto o stock insuficiente)</response>
        /// <response code="404">Orden no encontrada</response>
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmOrder(Guid id)
        {
            _logger.LogInformation("Confirmando la orden con ID: {OrderId}", id);

            var command = new ConfirmOrderCommand(id);
            var result = await _mediator.SendAsync(command);

            if (result.IsFailure)
            {
                _logger.LogWarning("Error al confirmar la orden {OrderId}: {Error}", id, result.Error);

                if (result.Error.Contains("not found"))
                    return NotFound(new { error = result.Error });

                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Orden {OrderId} confirmada satisfactoriamente", id);
            return NoContent();
        }

        /// <summary>
        /// Intenta enviar la orden con el ID especificado.
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <returns>Confirmación exitosa</returns>
        /// <response code="204">Orden enviada correctamente</response>
        /// <response code="400">No se puede enviar orden (no está confirmada)</response>
        /// <response code="404">Orden no encontrada</response>
        [HttpPost("{id}/ship")]
        public async Task<IActionResult> ShipOrder(Guid id)
        {
            _logger.LogInformation("Intentando enviar la orden con ID: {OrderId}", id);

            // TODO: Implementar ShipOrderCommand
            return StatusCode(StatusCodes.Status501NotImplemented,
                new { message = "Ship order feature will be implemented soon" });
        }

        /// <summary>
        /// Intenta entregar la orden con el ID especificado.
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <returns>Confirmación exitosa</returns>
        /// <response code="204">Orden entregada correctamente</response>
        /// <response code="400">No se puede entregar la orden (no está enviada)</response>
        /// <response code="404">Orden no encontrada</response>
        [HttpPost("{id}/deliver")]
        public async Task<IActionResult> DeliverOrder(Guid id)
        {
            _logger.LogInformation("Intentando entregar la orden con ID: {OrderId}", id);

            // TODO: Implement DeliverOrderCommand
            return StatusCode(StatusCodes.Status501NotImplemented,
                new { message = "Deliver order feature will be implemented soon" });
        }
    }
}

#region

public class CancelOrderRequest
{
    /// <summary>
    /// Razón para la cancelación de la orden.
    /// </summary>
    public string Reason { get; set; }
}

#endregion