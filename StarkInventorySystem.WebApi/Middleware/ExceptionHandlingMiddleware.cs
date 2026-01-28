using FluentValidation;
using StarkInventorySystem.Domain.DomainExceptions;
using System.Net;
using System.Text.Json;

namespace StarkInventorySystem.WebApi.Middleware
{
    /// <summary>
    /// Middleware para el manejo global de excepciones en la API.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invoca el middleware y maneja las excepciones no controladas.
        /// </summary>
        /// <param name="context">Contexto Http</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Maneja la excepción y escribe una respuesta de error adecuada.
        /// </summary>
        /// <param name="context">Contexto Http</param>
        /// <param name="exception">La excepción capturada</param>
        /// <returns> </returns>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Loggear la excepción
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            // Determinar el status code y la respuesta de error basado en el tipo de excepción
            var (statusCode, errorResponse) = exception switch
            {
                // Excepción de dominio (Romper una regla de negocio) -> 400 Bad Request
                DomainException domainEx => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Una regla de negocio fue rota.",
                        Details = domainEx.Message
                    }
                ),

                // Excepciones de validación (Fallos en input validations)
                ValidationException validationEx => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Errores de validación en la solicitud.",
                        Details = string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage)),
                        Errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList()
                    }
                ),

                // Excepción de no encontrado -> 404 Not Found
                KeyNotFoundException => (
                    HttpStatusCode.NotFound,
                    new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = "Recurso no encontrado.",
                        Details = exception.Message
                    }
                ),


                // Excepción de no autorizado -> 401 Unauthorized
                UnauthorizedAccessException => (
                    HttpStatusCode.Unauthorized,
                    new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Message = "Acceso no autorizado.",
                        Details = exception.Message
                    }
                ),

                // Excepción genérica -> 500 Internal Server Error
                _ => (
                    HttpStatusCode.InternalServerError,
                    new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Message = "Ocurrió un error interno en el servidor.",
                        Details = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                        ? exception.Message // Mensaje detallado en desarrollo
                        : "Un error ocurrió mientras se procesaba su petición" // Mensaje genérico en producción
                    }
                )
            };

            // Configurar la respuesta HTTP
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            // Serializar y escribir la respuesta de error
            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

    }
}
