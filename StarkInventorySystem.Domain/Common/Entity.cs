using StarkInventorySystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.Common
{
    public abstract class Entity : IEquatable<Entity>
    {
        public Guid Id { get; protected set; }

        // Colección de eventos de dominio asociados a la entidad (Se implementará luego).
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();


        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        protected void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        // Implementación de IEquatable<Entity>. Igualdad basada en el Id.
        public bool Equals(Entity? other)
        {
            if (other is null) return false; // La entidad no puede ser nula.
            if (ReferenceEquals(this, other)) return true; // Mismo objeto en memoria.
            if (GetType() != other.GetType()) return false; // Diferente tipo de entidad.
            return Id == other.Id; // Igualdad basada en el Id.
        }

        //
        public override bool Equals(object? obj)
        {
            return obj is Entity entity && Equals(entity); // Reutiliza la implementación de IEquatable<Entity>.
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode(); // Hash code basado en el Id.
        }

        public static bool operator ==(Entity? left, Entity? right)
        {
            return left?.Equals(right) ?? right is null; // Maneja la igualdad nula.
        }

        public static bool operator !=(Entity? left, Entity? right)
        {
            return !(left == right); // Reutiliza el operador ==.
        }
    }
}
