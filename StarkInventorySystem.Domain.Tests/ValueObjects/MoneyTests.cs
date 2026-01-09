using StarkInventorySystem.Domain.DomainExceptions;
using StarkInventorySystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.Tests.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Create_WithValidParameters_ReturnsMoney()
        {
            // Arrange & Act
            var money = Money.Create(100.50m, "USD");

            // Assert
            Assert.NotNull(money);
            Assert.Equal(100.50m, money.Amount);
            Assert.Equal("USD", money.Currency);
        }

        [Fact]
        public void Create_WithLowercaseCurrency_ConvertsToUppercase()
        {
            // Arrange & Act
            var money = Money.Create(50.00m, "eur");
            // Assert
            Assert.Equal("EUR", money.Currency);
        }

        [Fact]
        public void Create_WithNegativeAmount_ThrowsDomainException()
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<DomainException>(() => Money.Create(-10.00m, "USD"));
            Assert.Equal("La cantidad en Money no puede ser un número negativo.", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Create_WithEmptyCurrency_ThrowsDomainException(string invalidCurrency)
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<DomainException>(() => Money.Create(100m, invalidCurrency));
            Assert.Equal("El tipo de moneda en Money no puede estar vacía.", exception.Message);
        }

        [Theory]
        [InlineData("US")]
        [InlineData("EURO")]
        public void Create_WithInvalidCurrencyLength_ThrowsDomainException(string invalidCurrency)
        {
            // Arrange, Act & Assert
            var exception = Assert.Throws<DomainException>(() => Money.Create(100m, invalidCurrency));
            Assert.Equal("El tipo de moneda debe tener un código ISO de 3 letras (e.g., USD, EUR, COP).", exception.Message);
        }

        [Fact]
        public void Add_WithSameCurrency_ReturnsNewMoneyWithSum()
        {
            // Arrange
            var money1 = Money.Create(100m, "USD");
            var money2 = Money.Create(50m, "USD");

            // Act
            var result = money1.Add(money2);

            // Assert
            Assert.Equal(150m, result.Amount);
            Assert.Equal("USD", result.Currency);
        }

        [Fact]
        public void Add_WithDifferentCurrencies_ThrowsDomainException()
        {
            // Arrange
            var money1 = Money.Create(100m, "USD");
            var money2 = Money.Create(50m, "EUR");
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => money1.Add(money2));
            Assert.Equal("No se pueden sumar cantidades en diferentes tipos de monedas. Tipo de moneda: USD y EUR", exception.Message);
        }

        [Fact]
        public void Add_WithNull_ThrowsDomainException()
        {
            // Arrange
            var money1 = Money.Create(100m, "USD");

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => money1.Add(null));
            Assert.Equal("No se puede añadir dinero nulo.", exception.Message);
        }

        [Fact]
        public void Substract_WithSameCurrency_ReturnsNewMoneyWithDifference()
        {
            // Arrange
            var money1 = Money.Create(100m, "USD");
            var money2 = Money.Create(60m, "USD");

            // Act
            var result = money1.Substract(money2);

            // Assert
            Assert.Equal(40m, result.Amount);
            Assert.Equal("USD", result.Currency);
        }

        [Fact]
        public void Substract_ResultingInNegative_ThrowsDomainException()
        {
            // Arrange
            var money1 = Money.Create(50m, "USD");
            var money2 = Money.Create(100m, "USD");

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => money1.Substract(money2));
            Assert.Equal("El resultado de la resta no puede ser un número negativo.", exception.Message);
        }

        [Fact]
        public void Multiply_WithPositiveFactor_ReturnsProduct()
        {
            // Arrange
            var money = Money.Create(50m, "USD");

            // Act
            var result = money.Multiply(2);

            // Assert
            Assert.Equal(100m, result.Amount);
            Assert.Equal("USD", result.Currency);
        }

        [Fact]
        public void Multiply_WithZeroFactor_ReturnsZero()
        {
            // Arrange
            var money = Money.Create(50m, "USD");

            // Act
            var result = money.Multiply(0);

            // Assert
            Assert.Equal(0m, result.Amount);
        }

        [Fact]
        public void Multiply_WithNegativeFactor_ThrowsDomainException()
        {
            // Arrange
            var money = Money.Create(50m, "USD");

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => money.Multiply(-2));
            Assert.Equal("No se puede multiplicar por un factor negativo.", exception.Message);

        }

        [Fact]
        public void IsGreaterThan_WhenGreater_ReturnsTrue()
        {
            // Arrange
            var money1 = Money.Create(100m, "USD");
            var money2 = Money.Create(50m, "USD");
            // Act
            var result = money1.IsGreaterThan(money2);
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsGreaterThan_WhenLess_ReturnsFalse()
        {
            // Arrange
            var money1 = Money.Create(50m, "USD");
            var money2 = Money.Create(100m, "USD");
            // Act
            var result = money1.IsGreaterThan(money2);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsLessThan_WhenLess_ReturnsTrue()
        {
            // Arrange
            var money1 = Money.Create(50m, "USD");
            var money2 = Money.Create(100m, "USD");
            // Act
            var result = money1.IsLessThan(money2);
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsLessThan_WhenGreater_ReturnsFalse()
        {
            // Arrange
            var money1 = Money.Create(100m, "USD");
            var money2 = Money.Create(50m, "USD");
            // Act
            var result = money1.IsLessThan(money2);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithSameValues_ReturnsTrue()
        {
            // Arrange
            var money1 = Money.Create(100m, "USD");
            var money2 = Money.Create(100m, "USD");

            // Act & Assert
            Assert.Equal(money1, money2);
            Assert.True(money1 == money2);
        }

        [Fact]
        public void Equals_WithDifferentAmounts_ReturnsFalse()
        {
            // Arrange
            var money1 = Money.Create(100m, "USD");
            var money2 = Money.Create(50m, "USD");

            // Act & Assert
            Assert.NotEqual(money1, money2);
        }

        [Fact]
        public void Equals_WithDifferentCurrencies_ReturnsFalse()
        {
            // Arrange
            var money1 = Money.Create(100m, "USD");
            var money2 = Money.Create(100m, "EUR");

            // Act & Assert
            Assert.NotEqual(money1, money2);
        }

        [Fact]
        public void ToString_FormatsCorrectly()
        {
            // Arrange
            var money = Money.Create(1234.56m, "COP");

            // Act
            var result = money.ToString();

            // Assert
            Assert.Equal("1.234,56 COP", result);
        }

        [Fact]
        public void Zero_CreatesZeroMoney()
        {
            // Act
            var money = Money.Zero("USD");

            // Assert
            Assert.Equal(0m, money.Amount);
            Assert.Equal("USD", money.Currency);
        }

        [Fact]
        public void Usd_CreatesUsdMoney()
        {
            // Act
            var money = Money.Usd(100m);

            // Assert
            Assert.Equal(100m, money.Amount);
            Assert.Equal("USD", money.Currency);
        }

        [Fact]
        public void Eur_CreatesEurMoney()
        {
            // Act
            var money = Money.Eur(100m);

            // Assert
            Assert.Equal(100m, money.Amount);
            Assert.Equal("EUR", money.Currency);
        }

        [Fact]
        public void Cop_CreatesCopMoney()
        {
            // Act
            var money = Money.Cop(100m);

            // Assert
            Assert.Equal(100m, money.Amount);
            Assert.Equal("COP", money.Currency);
        }
    }
}
