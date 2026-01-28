using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StarkInventorySystem.Domain.Entities;
using StarkInventorySystem.Domain.Enums;
using StarkInventorySystem.Domain.ValueObjects;
using StarkInventorySystem.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkInventorySystem.Infrastructure.Services
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Comenzando populación de la base de datos...");

                // Verificar si la base de datos ya tiene datos
                if (await _context.Products.AnyAsync())
                {
                    _logger.LogInformation("La base de datos ya contiene datos. Saltando la populación.");
                    return;
                }

                // Orden populación: Productos primero, luego Órdenes
                var products = await SeedProductsAsync();
                await SeedOrdersAsync(products);

                _logger.LogInformation("Populación de la base de datos completada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error ocurrió al popular la base de datos");
                throw;
            }
        }

        private async Task<List<Product>> SeedProductsAsync()
        {
            _logger.LogInformation("Populando productos...");

            var products = new List<Product>();

            // Electronics - High Value Items
            products.Add(CreateProduct(
                "Dell XPS 15 Laptop",
                "LAPTOP-DELL-XPS15",
                1299.99m,
                "15-inch laptop with Intel i7, 16GB RAM, 512GB SSD",
                50,
                10
            ));

            products.Add(CreateProduct(
                "MacBook Pro 14-inch",
                "LAPTOP-APPLE-MBP14",
                1999.99m,
                "M3 chip, 16GB unified memory, 512GB SSD",
                30,
                5
            ));

            products.Add(CreateProduct(
                "iPhone 15 Pro",
                "PHONE-APPLE-IP15PRO",
                999.99m,
                "6.1-inch display, A17 Pro chip, 128GB",
                100,
                20
            ));

            products.Add(CreateProduct(
                "Samsung Galaxy S24",
                "PHONE-SAMSUNG-S24",
                899.99m,
                "6.2-inch display, Snapdragon 8 Gen 3, 256GB",
                75,
                15
            ));

            // Peripherals - Medium Value
            products.Add(CreateProduct(
                "Logitech MX Master 3S",
                "MOUSE-LOGI-MXM3S",
                99.99m,
                "Wireless ergonomic mouse with customizable buttons",
                200,
                30
            ));

            products.Add(CreateProduct(
                "Mechanical Keyboard RGB",
                "KEYBOARD-MECH-RGB",
                149.99m,
                "Cherry MX switches, RGB backlight, USB-C",
                150,
                25
            ));

            products.Add(CreateProduct(
                "4K Webcam HD Pro",
                "WEBCAM-4K-HDPRO",
                199.99m,
                "4K resolution, auto-focus, noise cancellation mic",
                80,
                15
            ));

            // Accessories - Lower Value (Some with LOW STOCK for testing)
            products.Add(CreateProduct(
                "USB-C Hub 7-in-1",
                "HUB-USBC-7IN1",
                49.99m,
                "7 ports: HDMI, USB 3.0, SD card, ethernet",
                8, // LOW STOCK!
                10
            ));

            products.Add(CreateProduct(
                "Wireless Charger 15W",
                "CHARGER-WIRELESS-15W",
                29.99m,
                "Fast wireless charging pad, Qi-compatible",
                5, // LOW STOCK!
                10
            ));

            products.Add(CreateProduct(
                "Laptop Stand Aluminum",
                "STAND-LAPTOP-ALU",
                39.99m,
                "Adjustable aluminum laptop stand, ergonomic",
                120,
                20
            ));

            // Software/Digital Products
            products.Add(CreateProduct(
                "Office Suite Pro License",
                "SOFTWARE-OFFICE-PRO",
                79.99m,
                "1-year license for word processor, spreadsheet, presentations",
                500,
                50
            ));

            products.Add(CreateProduct(
                "Antivirus Premium",
                "SOFTWARE-ANTIVIRUS",
                49.99m,
                "Premium antivirus protection, 3 devices, 1 year",
                1000,
                100
            ));

            // Gaming
            products.Add(CreateProduct(
                "Gaming Headset 7.1",
                "HEADSET-GAMING-71",
                129.99m,
                "7.1 surround sound, noise cancelling mic, RGB",
                3, // LOW STOCK!
                15
            ));

            products.Add(CreateProduct(
                "Gaming Mouse Pad XL",
                "MOUSEPAD-GAMING-XL",
                24.99m,
                "Extended size, non-slip rubber base, smooth surface",
                250,
                40
            ));

            products.Add(CreateProduct(
                "Monitor 27-inch 4K",
                "MONITOR-27-4K",
                449.99m,
                "27-inch IPS, 4K UHD, HDR10, 60Hz",
                40,
                8
            ));

            // Add all products to context
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Populados {Count} productos", products.Count);
            return products;
        }

        private async Task<List<Order>> SeedOrdersAsync(List<Product> products)
        {
            _logger.LogInformation("Seeding orders...");

            var orders = new List<Order>();

            // Sample customer IDs (in real app, these would be from Users table)
            var customer1 = Guid.NewGuid();
            var customer2 = Guid.NewGuid();
            var customer3 = Guid.NewGuid();

            // Order 1: PENDING - Just created, not confirmed yet
            var order1 = CreateOrder(
                customer1,
                "123 Tech Street",
                "San Francisco",
                "CA",
                "94105",
                "USA"
            );
            order1.AddItem(products[0], 2); // Dell XPS 15 x2
            order1.AddItem(products[4], 1); // Logitech Mouse x1
            orders.Add(order1);

            // Order 2: CONFIRMED - Stock reserved
            var order2 = CreateOrder(
                customer2,
                "456 Innovation Ave",
                "New York",
                "NY",
                "10001",
                "USA"
            );
            order2.AddItem(products[2], 1); // iPhone 15 Pro x1
            order2.AddItem(products[10], 1); // Office Suite x1
            orders.Add(order2);

            // Order 3: SHIPPED - On the way to customer
            var order3 = CreateOrder(
                customer3,
                "789 Enterprise Blvd",
                "Austin",
                "TX",
                "73301",
                "USA"
            );
            order3.AddItem(products[1], 1); // MacBook Pro x1
            order3.AddItem(products[5], 1); // Mechanical Keyboard x1
            orders.Add(order3);

            // Order 4: DELIVERED - Successfully completed
            var order4 = CreateOrder(
                customer1,
                "123 Tech Street",
                "San Francisco",
                "CA",
                "94105",
                "USA"
            );
            order4.AddItem(products[6], 1); // 4K Webcam x1
            order4.AddItem(products[9], 2); // Laptop Stand x2
            orders.Add(order4);

            // Order 5: CANCELLED - Customer cancelled
            var order5 = CreateOrder(
                customer2,
                "456 Innovation Ave",
                "New York",
                "NY",
                "10001",
                "USA"
            );
            order5.AddItem(products[3], 1); // Samsung Galaxy S24 x1
            orders.Add(order5);

            // ⚠️ IMPORTANT: Save orders FIRST so they get IDs
            await _context.Orders.AddRangeAsync(orders);
            await _context.SaveChangesAsync();

            // NOW we can manually adjust status and reserve stock
            // Order 2: Make it Confirmed
            products[2].RemoveStock(1);   // iPhone stock
            products[10].RemoveStock(1);  // Office Suite stock
            SetOrderStatus(order2, OrderStatus.Confirmed, confirmedAt: DateTime.UtcNow.AddDays(-2));

            // Order 3: Make it Shipped
            products[1].RemoveStock(1);   // MacBook stock
            products[5].RemoveStock(1);   // Keyboard stock
            SetOrderStatus(order3, OrderStatus.Shipped,
                confirmedAt: DateTime.UtcNow.AddDays(-5),
                shippedAt: DateTime.UtcNow.AddDays(-3));

            // Order 4: Make it Delivered
            products[6].RemoveStock(1);   // Webcam stock
            products[9].RemoveStock(2);   // Laptop Stand stock
            SetOrderStatus(order4, OrderStatus.Delivered,
                confirmedAt: DateTime.UtcNow.AddDays(-10),
                shippedAt: DateTime.UtcNow.AddDays(-8),
                deliveredAt: DateTime.UtcNow.AddDays(-5));

            // Order 5: Make it Cancelled
            SetOrderStatus(order5, OrderStatus.Cancelled,
                cancelledAt: DateTime.UtcNow.AddDays(-1),
                cancellationReason: "El usuario cambió de parecer");

            // Save the status changes and stock updates
            await _context.SaveChangesAsync();

            _logger.LogInformation("Populada con {Count} ordenes con varios estados", orders.Count);
            return orders;
        }

        #region Helper Methods
        private Product CreateProduct(
            string name,
            string sku,
            decimal price,
            string description,
            int initialStock,
            int lowStockThreshold
            )
        {
            var product = Product.Create(
                name,
                sku,
                Money.Usd(price),
                description
            );

            product.AddStock(initialStock);
            product.SetLowStockThreshold(lowStockThreshold);

            return product;
        }

        private Order CreateOrder(
            Guid customerId,
            string street,
            string city,
            string state,
            string postalCode,
            string country
            )
        {
            var address = Address.Create(
                street,
                city,
                state,
                postalCode,
                country
            );
            return Order.Create(customerId, address);
        }

        /// <summary>
        /// Método helper para asignar el order status usando reflexión (Para popular la BBDD sólamente)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="status"></param>
        /// <param name="confirmedAt"></param>
        /// <param name="shippedAt"></param>
        /// <param name="deliveredAt"></param>
        /// <param name="cancelledAt"></param>
        /// <param name="cancellationReason"></param>
        private void SetOrderStatus(
            Order order,
            OrderStatus status,
            DateTime? confirmedAt = null,
            DateTime? shippedAt = null,
            DateTime? deliveredAt = null,
            DateTime? cancelledAt = null,
            string cancellationReason = null)
        {
            typeof(Order).GetProperty("Status")!.SetValue(order, status);

            if (confirmedAt.HasValue)
                typeof(Order).GetProperty("ConfirmedAt")!.SetValue(order, confirmedAt.Value);

            if (shippedAt.HasValue)
                typeof(Order).GetProperty("ShippedAt")!.SetValue(order, shippedAt.Value);

            if (deliveredAt.HasValue)
                typeof(Order).GetProperty("DeliveredAt")!.SetValue(order, deliveredAt.Value);

            if (cancelledAt.HasValue)
                typeof(Order).GetProperty("CancelledAt")!.SetValue(order, cancelledAt.Value);

            if (!string.IsNullOrEmpty(cancellationReason))
                typeof(Order).GetProperty("CancellationReason")!.SetValue(order, cancellationReason);
        }
        #endregion
    }
}