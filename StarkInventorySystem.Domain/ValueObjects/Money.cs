using StarkInventorySystem.Domain.DomainExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.ValueObjects
{
    public sealed record Money
    {
        public decimal Amount { get; init; } // Cantidad monetaria
        public string Currency { get; init; } // Tipo de moneda (e.g., USD, EUR, COP)

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        // Factory method para crear instancias de Money con validaciones.
        public static Money Create(decimal amount, string currency)
        {
            if (amount < 0)
            {
                throw new DomainException("La cantidad en Money no puede ser un número negativo.");
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new DomainException("El tipo de moneda en Money no puede estar vacía.");
            }

            if (currency.Length != 3)
            {
                throw new DomainException("El tipo de moneda debe tener un código ISO de 3 letras (e.g., USD, EUR, COP).");
            }

            return new Money(amount, currency.ToUpperInvariant());
        }

        // Operaciones aritméticas y de comparación.
        public Money Add(Money other)
        {
            if (other is null)
            {
                throw new DomainException("No se puede añadir dinero nulo.");
            }

            if (Currency != other.Currency)
            {
                throw new DomainException($"No se pueden sumar cantidades en diferentes tipos de monedas. Tipo de moneda: {Currency} y {other.Currency}");
            }

            return new Money(Amount + other.Amount, Currency);
        }

        // Resta la cantidad de otro Money del actual.
        public Money Substract(Money other)
        {
            if (other is null)
            {
                throw new DomainException("No se puede restar dinero nulo.");
            }

            if (Currency != other.Currency)
            {
                throw new DomainException($"No se pueden restar cantidades en diferentes tipos de monedas. Tipo de moneda: {Currency} y {other.Currency}");
            }

            var result = Amount - other.Amount;

            if (result < 0)
            {
                throw new DomainException("El resultado de la resta no puede ser un número negativo.");
            }

            return new Money(result, Currency);
        }

        // Multiplica la cantidad por un factor decimal.
        public Money Multiply(decimal factor)
        {
            if (factor < 0)
            {
                throw new DomainException("No se puede multiplicar por un factor negativo.");
            }

            return new Money(Amount * factor, Currency);
        }

        // Compara si esta cantidad es mayor que otra.
        public bool IsGreaterThan(Money other)
        {
            if (other is null)
            {
                throw new DomainException("No se puede hacer una comparación con dinero nulo.");
            }

            if (Currency != other.Currency)
            {
                throw new DomainException($"No se pueden restar cantidades en diferentes tipos de monedas. Tipo de moneda: {Currency} y {other.Currency}");
            }

            return Amount > other.Amount;
        }

        // Compara si esta cantidad es menor que otra.
        public bool IsLessThan(Money other)
        {
            if (other is null)
            {
                throw new DomainException("No se puede hacer una comparación con dinero nulo.");
            }

            if (Currency != other.Currency)
            {
                throw new DomainException($"No se pueden restar cantidades en diferentes tipos de monedas. Tipo de moneda: {Currency} y {other.Currency}");
            }

            return Amount < other.Amount;
        }

        // Override a ToString para mejor legibilidad.
        public override string ToString() => $"{Amount:N2} {Currency}";

        // Helpers estáticos para crear Money en tipos de monedas comunes.
        public static Money Zero(string currency) => new(0, currency); // Cantidad cero en la moneda especificada
        public static Money Usd(decimal amount) => Create(amount, "USD"); // Helper para dólares estadounidenses
        public static Money Eur(decimal amount) => Create(amount, "EUR"); // Helper para euros
        public static Money Cop(decimal amount) => Create(amount, "COP"); // Helper para pesos colombianos
    }
}
