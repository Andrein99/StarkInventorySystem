using StarkInventorySystem.Domain.DomainExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.ValueObjects
{
    // Objeto de valor que representa una dirección física.
    // Inmutable y validado en el momento de la creación.
    public sealed record Address
    {
        public string Street { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string PostalCode { get; init; }
        public string Country { get; init; }

        private Address(string street, string city, string state, string postalCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }

        // Fábrica para crear una nueva instancia de Address con validación.
        public static Address Create(string street, string city, string state, string postalCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
            {
                throw new DomainException("La dirección de la calle no puede estar vacía.");
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                throw new DomainException("La ciudad no puede estar vacía.");
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                throw new DomainException("El estado no puede estar vacío.");
            }

            if (string.IsNullOrWhiteSpace(postalCode))
            {
                throw new DomainException("El código postal no puede estar vacío.");
            }

            if (string.IsNullOrWhiteSpace(country))
            {
                throw new DomainException("El país no puede estar vacío.");
            }

            return new Address(street.Trim(), city.Trim(), state.Trim().ToUpperInvariant(), postalCode.Trim(), country.Trim().ToUpperInvariant());
        }

        // Método para obtener la dirección completa como una cadena formateada.
        public string GetFullAddress()
        {
            return $"{Street}, {City}, {State}, {PostalCode}, {Country}";
        }

        public override string ToString() => GetFullAddress();
    }
}
