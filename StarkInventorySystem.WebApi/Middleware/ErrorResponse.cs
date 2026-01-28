namespace StarkInventorySystem.WebApi.Middleware
{
    /// <summary>
    /// Estructura estándar para las respuestas de error de la API.
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
