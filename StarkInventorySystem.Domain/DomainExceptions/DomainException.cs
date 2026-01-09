using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.DomainExceptions
{
    // Excepción personalizada para errores de dominio.
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) // Constructor que acepta un mensaje de error
        {
        }
    
        public DomainException(string message, Exception innerException) : base(message, innerException) // Agrega soporte para excepciones internas
        {
        }
    }
}
