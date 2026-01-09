using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.Interfaces
{
    public interface IDomainEvent // Interfaz para eventos de dominio
    {
        Guid EventId { get; } // Identificador único del evento
        DateTime OccurredOn { get; } // Fecha y hora en que ocurrió el evento
    }
}
