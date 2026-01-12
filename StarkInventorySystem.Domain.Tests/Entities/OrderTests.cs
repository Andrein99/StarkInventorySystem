using StarkInventorySystem.Domain.DomainExceptions;
using StarkInventorySystem.Domain.Entities;
using StarkInventorySystem.Domain.Enums;
using StarkInventorySystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Domain.Tests.Entities
{
    public class OrderTests
    {
        #region Order Creation Tests
        
        [Fact]
        public void Create_WithValidParameters_ReturnsOrder()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var shippingAddress = CreateValidAddress();

            // Act
            var order = Order.Create(customerId, shippingAddress);

            // Assert
            Assert.NotNull(order);
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal(customerId, order.CustomerId);
            Assert.Equal(shippingAddress, order.ShippingAddress);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Empty(order.Items);
            Assert.Equal(Money.Zero("USD"), order.Total);
            Assert.NotEqual(default, order.CreatedAt);
        }

        [Fact]
        public void Create_WithEmptyCustomerId_ThrowsDomainException()
        {
            // Arrange
            var customerId = Guid.Empty;
            var shippingAddress = CreateValidAddress();
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => Order.Create(customerId, shippingAddress));
            Assert.Equal("El Id del cliente no puede estar vacío.", exception.Message);
        }

        [Fact]
        public void Create_WithNullShippingAddress_ThrowsDomainException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => Order.Create(customerId, null));
            Assert.Equal("La dirección de envío no puede ser nula.", exception.Message);
        }

        #endregion

        #region Add Item Tests

        [Fact]
        public void AddItem_WithValidProduct_AddsItemAndRecalculatesTotal()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m);
            var quantity = 2;

            // Act
            order.AddItem(product, quantity);

            // Assert
            Assert.Single(order.Items);
            var item = order.Items.First();
            Assert.Equal(product.Id, item.ProductId);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(product.Price, item.UnitPrice);
            Assert.Equal(Money.Usd(4000m), order.Total);
        }

        [Fact]
        public void AddItem_MultipleProducts_CalculatesTotalCorrectly()
        {
            // Arrange
            var order = CreateValidOrder();
            var product1 = CreateValidProduct("Laptop", 2000m);
            var product2 = CreateValidProduct("Mouse", 50m);

            // Act
            order.AddItem(product1, 2); // 4000
            order.AddItem(product2, 3); // 150

            // Assert
            Assert.Equal(2, order.Items.Count);
            Assert.Equal(Money.Usd(4150m), order.Total);
        }

        [Fact]
        public void AddItem_SameProductTwice_CreatesMultipleLineItems()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m);

            // Act
            order.AddItem(product, 1);
            order.AddItem(product, 2);

            // Assert
            Assert.Equal(2, order.Items.Count);
            Assert.Equal(Money.Usd(6000m), order.Total);
        }

        [Fact]
        public void AddItem_WithNullProduct_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.AddItem(null, 1));
            Assert.Equal("El producto no puede ser nulo.", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void AddItem_WithNonPositiveQuantity_ThrowsDomainException(int invalidQuantity)
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.AddItem(product, invalidQuantity));
            Assert.Equal("La cantidad de artículos en la orden debe ser un número positivo.", exception.Message);
        }

        [Fact]
        public void AddItem_ToConfirmedOrder_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m);
            order.AddItem(product, 1);
            order.Confirm();

            var anotherProduct = CreateValidProduct("Mouse", 50m);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.AddItem(anotherProduct, 1));
            Assert.Equal("Solo se pueden modificar órdenes en estado pendiente.", exception.Message);
        }

        [Fact]
        public void AddItem_InactiveProduct_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m);
            product.Deactivate();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.AddItem(product, 1));
            Assert.Equal("No se puede agregar un producto inactivo a la orden.", exception.Message);
        }

        [Fact]
        public void AddItem_InsufficientStock_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m, 5);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.AddItem(product, 6));
            Assert.Equal("No hay stock suficiente para el producto Laptop. Stock disponible: 5, cantidad solicitada: 6", exception.Message);
        }

        #endregion

        #region Remove Item Tests

        [Fact]
        public void RemoveItem_WithValidOrderItemId_RemovesItemAndRecalculatesTotal()
        {
            // Arrange
            var order = CreateValidOrder();
            var product1 = CreateValidProduct("Laptop", 2000m);
            var product2 = CreateValidProduct("Mouse", 50m);
            order.AddItem(product1, 1); // 2000
            order.AddItem(product2, 1); // 50

            var itemToRemove = order.Items.First(i => i.ProductId == product2.Id);

            // Act
            order.RemoveItem(itemToRemove.Id);

            // Assert
            Assert.Single(order.Items);
            Assert.Equal(Money.Usd(2000m), order.Total);
        }

        [Fact]
        public void RemoveItem_LastItem_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m);
            order.AddItem(product, 1); // 2000
            var itemId = order.Items.First().Id;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.RemoveItem(itemId));
            Assert.Equal("No se puede eliminar el último artículo de la orden. Una orden debe tener al menos un artículo.", exception.Message);
        }

        [Fact]
        public void RemoveItem_NonExistentItem_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m);
            order.AddItem(product, 1); // 2000
            var nonExistentItemId = Guid.NewGuid();
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.RemoveItem(nonExistentItemId));
            Assert.Equal("El ítem a eliminar no existe en la orden.", exception.Message);
        }

        [Fact]
        public void RemoveItem_FromConfirmedOrder_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product1 = CreateValidProduct("Laptop", 2000m);
            var product2 = CreateValidProduct("Mouse", 50m);
            order.AddItem(product1, 1); // 2000
            order.AddItem(product2, 1); // 50
            order.Confirm();
            var itemId = order.Items.First().Id;
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.RemoveItem(itemId));
            Assert.Equal("Solo se pueden modificar órdenes en estado pendiente.", exception.Message);
        }

        #endregion

        #region Order Status Transition Tests

        [Fact]
        public void Confirm_WithPendingOrder_ChangesStatusToConfirmed()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m);
            order.AddItem(product, 1);
            // Act
            order.Confirm();
            // Assert
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            Assert.NotNull(order.ConfirmedAt);
        }

        [Fact]
        public void Confirm_WithEmptyOrder_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.Confirm());
            Assert.Equal("No se puede confirmar una orden sin artículos.", exception.Message);
        }

        [Fact]
        public void Confirm_AlreadyConfirmedOrder_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 2000m);
            order.AddItem(product, 1);
            order.Confirm();
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.Confirm());
            Assert.Equal("Solo se pueden modificar órdenes en estado pendiente.", exception.Message);
        }

        [Fact]
        public void Ship_WithConfirmedOrder_ChangesStatusToShipped()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 1000m);
            order.AddItem(product, 1);
            order.Confirm();

            // Act
            order.Ship();

            // Assert
            Assert.Equal(OrderStatus.Shipped, order.Status);
            Assert.NotNull(order.ShippedAt);
        }

        [Fact]
        public void Ship_WithPendingOrder_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 1000m);
            order.AddItem(product, 1);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                order.Ship());

            Assert.Equal("La orden debe estar confirmada antes de ser enviada.", exception.Message);
        }

        [Fact]
        public void Deliver_WithShippedOrder_ChangesStatusToDelivered()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 1000m);
            order.AddItem(product, 1);
            order.Confirm();
            order.Ship();

            // Act
            order.Deliver();

            // Assert
            Assert.Equal(OrderStatus.Delivered, order.Status);
            Assert.NotNull(order.DeliveredAt);
        }

        [Fact]
        public void Deliver_WithNonShippedOrder_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 1000m);
            order.AddItem(product, 1);
            order.Confirm();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                order.Deliver());

            Assert.Equal("Solo se pueden entregar órdenes en estado enviado.", exception.Message);
        }

        [Fact]
        public void Cancel_WithPendingOrder_ChangesStatusToCancelled()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 1000m);
            order.AddItem(product, 1);

            // Act
            order.Cancel("El cliente solicitó cancelación");

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.NotNull(order.CancelledAt);
            Assert.Equal("El cliente solicitó cancelación", order.CancellationReason);
        }

        [Fact]
        public void Cancel_WithShippedOrder_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 1000m);
            order.AddItem(product, 1);
            order.Confirm();
            order.Ship();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                order.Cancel("Muy tarde"));

           Assert.Equal("No se pueden cancelar órdenes que ya han sido enviadas o entregadas.", exception.Message);
        }

        [Fact]
        public void Cancel_WithoutReason_ThrowsDomainException()
        {
            // Arrange
            var order = CreateValidOrder();
            var product = CreateValidProduct("Laptop", 1000m);
            order.AddItem(product, 1);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() =>
                order.Cancel(""));

            Assert.Equal("La razón de cancelación no puede estar vacía.", exception.Message);
        }

        #endregion

        #region Métodos Helper 

        private Order CreateValidOrder()
        {
            var customerId = Guid.NewGuid();
            var shippingAddress = CreateValidAddress();
            return Order.Create(
                customerId,
                shippingAddress
            );
        }

        private Product CreateValidProduct(string name, decimal price, int initialStock = 100)
        {
            var product = Product.Create(
                name,
                $"SKU-{Guid.NewGuid().ToString()[..8]}",
                Money.Usd(price),
                $"Descripción del producto {name}"
            );
            product.AddStock(initialStock);
            return product;
        }

        private Address CreateValidAddress()
        {
            return Address.Create(
                "123 Main St",
                "New York",
                "NY",
                "10001",
                "USA"
            );
        }

        #endregion
    }
}
