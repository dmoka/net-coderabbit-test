using System;
using System.Threading.Tasks;
using Xunit;
using AntiPatternsDemo.Services;

namespace AntiPatternsDemo.Tests
{
    public class OrderServiceTests
    {
        private readonly OrderService _service = new();

        [Fact]
        public async Task Test1()
        {
            var order = await _service.ProcessOrder(
                "John Doe",
                "Laptop",
                1,
                999.99m,
                "123 Main St",
                "555-0123",
                "john@example.com",
                true,
                "Credit Card",
                "4111111111111111"
            );

            Assert.NotNull(order);
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal("John Doe", order.CustomerName);
            Assert.Equal("Laptop", order.ProductName);
            Assert.Equal(1, order.Quantity);
            Assert.Equal(999.99m, order.Price);
            Assert.Equal(999.99m, order.Total);
            Assert.Equal("Pending", order.Status);
            Assert.True((DateTime.Now - order.CreatedAt).TotalSeconds < 1);
        }

        [Fact]
        public async Task TestEmailSending()
        {
            var order = await _service.ProcessOrder(
                "Jane Doe",
                "Phone",
                2,
                499.99m,
                "456 Oak St",
                "555-0124",
                "jane@example.com",
                false,
                "PayPal",
                null
            );

            Assert.NotNull(order);
        }
    }
} 