using StarkInventorySystem.Domain.Entities;
using StarkInventorySystem.Domain.ValueObjects;
using StarkInventorySystem.Domain.DomainExceptions;

namespace StarkInventorySystem.Domain.Tests.Entities
{
    public class ProductTests
    {
        // Helper method para crear un product válido.
        private Product CreateValidProduct()
        {
            var name = "Laptop Dell XPS 15";
            var sku = "DELL-XPS15-001";
            var price = Money.Create(1300.01m, "USD");
            var description = "High performance laptop";
            return Product.Create(name, sku, price, description);
        }

        [Fact]
        public void Create_WithValidParameters_ReturnsProduct()
        {
            // Arrange
            var name = "Laptop Dell XPS 15";
            var sku = "DELL-XPS15-001";
            var price = Money.Create(1300.01m, "USD");
            var description = "High performance laptop";

            // Act
            var product = Product.Create(name, sku, price, description);

            // Assert
            Assert.NotNull(product);
            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.Equal(name, product.Name);
            Assert.Equal(sku, product.Sku);
            Assert.Equal(price, product.Price);
            Assert.Equal(description, product.Description);
            Assert.Equal(0, product.StockQuantity);
            Assert.True(product.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithInvalidName_ThrowsDomainException(string invalidName)
        {
            // Arrange
            var sku = "DELL-XPS15-001";
            var price = Money.Create(1300.01m, "USD");

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => Product.Create(invalidName, sku, price, "Description"));

            Assert.Equal("El nombre del producto no puede estar vacío.", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithInvalidSku_ThrowsDomainException(string invalidSku)
        {
            // Arrange
            var name = "Laptop Dell XPS 15";
            var price = Money.Create(1300.01m, "USD");

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => Product.Create(name, invalidSku, price, "Description"));

            Assert.Equal("El SKU del producto no puede estar vacío.", exception.Message);
        }

        [Fact]
        public void Create_WithNullPrice_ThrowDomainException()
        {
            // Arrange 
            var name = "Laptop Dell XPS 15";
            var sku = "DELL-XPS15-001";

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => Product.Create(name, sku, null, "Description"));
            Assert.Equal("El precio del producto no puede ser nulo.", exception.Message);
        }

        [Fact]
        public void AddStock_WithPositiveQuantity_IncreasesStock()
        {
            // Arrange
            var product = CreateValidProduct();
            var initialStock = product.StockQuantity;

            // Act
            product.AddStock(50);

            // Assert
            Assert.Equal(initialStock + 50, product.StockQuantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void AddStock_WithNonPositiveQuantity_ThrowsDomainException(int invalidQuantity)
        {
            // Arrange
            var product = CreateValidProduct();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => product.AddStock(invalidQuantity));
            Assert.Equal("La cantidad a agregar al stock debe ser mayor que cero.", exception.Message);
        }

        [Fact]
        public void RemoveStock_WithValidQuantity_DecreasesStock()
        {
            // Arrange
            var product = CreateValidProduct();
            product.AddStock(100);

            // Act
            product.RemoveStock(30);

            // Assert
            Assert.Equal(70, product.StockQuantity);
        }

        [Fact]
        public void RemoveStock_WithQuantityGreaterThanAvailable_ThrowsDomainException()
        {
            // Arrange
            var product = CreateValidProduct();
            product.AddStock(20);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => product.RemoveStock(21));
            Assert.Equal("No hay stock suficiente. Disponible: 20. Solicitado: 21", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void RemoveStock_WithNonPositiveQuantity_ThrowsDomainException(int invalidStock)
        {
            // Arrange
            var product = CreateValidProduct();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => product.RemoveStock(invalidStock));
            Assert.Equal("La cantidad a quitar del stock debe ser mayor que cero.", exception.Message);
        }

        [Fact]
        public void HasSufficientStock_WithEnoughStock_ReturnsTrue()
        {
            // Arrange
            var product = CreateValidProduct();
            product.AddStock(100);

            // Act
            var result = product.HasSufficientStock(50);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasSufficientStock_WithInsufficientStock_ReturnsFalse()
        {
            // Arrange
            var product = CreateValidProduct();
            product.AddStock(100);

            // Act
            var result = product.HasSufficientStock(101);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdatePrice_WithValidPrice_UpdatesPrice()
        {
            // Arrange
            var product = CreateValidProduct();
            var newPrice = Money.Create(1399.99m, "USD");

            // Act
            product.UpdatePrice(newPrice);

            // Assert
            Assert.Equal(newPrice, product.Price);
        }

        [Fact]
        public void UpdatePrice_WithNullPrice_ThrowsDomainException()
        {
            // Arrange
            var product = CreateValidProduct();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => product.UpdatePrice(null));

            Assert.Equal("El precio del producto no puede ser nulo.", exception.Message);
        }

        [Fact]
        public void Deactivate_ChangesIsActiveToFalse()
        {
            // Arrange
            var product = CreateValidProduct();
            // Act
            product.Deactivate();
            // Assert
            Assert.False(product.IsActive);
        }

        [Fact]
        public void Activate_ChangesIsActiveToTrue()
        {
            // Arrange
            var product = CreateValidProduct();
            // Act
            product.Activate();
            // Assert
            Assert.True(product.IsActive);
        }

        [Fact]
        public void SetLowStockThreshold_WithValidValue_SetsThreshold()
        {
            // Arrange
            var product = CreateValidProduct();

            // Act
            product.SetLowStockThreshold(10);

            // Assert
            Assert.Equal(10, product.LowStockThreshold);
        }

        [Fact]
        public void SetLowStockThreshold_WithNegativeThreshold_ThrowDomainException()
        {
            // Arrange
            var product = CreateValidProduct();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => product.SetLowStockThreshold(-5));

            Assert.Equal("El umbral de bajo stock no puede ser negativo.", exception.Message);
        }

        [Fact]
        public void IsLowStock_WhenStockBelowThreshold_ReturnsTrue()
        {
            // Arrange
            var product = CreateValidProduct();
            product.SetLowStockThreshold(20);
            product.AddStock(15);
            // Act
            var result = product.IsLowStock();
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsLowStock_WhenStockAboveThreshold_ReturnsFalse()
        {
            // Arrange
            var product = CreateValidProduct();
            product.SetLowStockThreshold(20);
            product.AddStock(25);
            // Act
            var result = product.IsLowStock();
            // Assert
            Assert.False(result);
        }
    }
}
