using System;
using System.Threading.Tasks;
using Xunit;
using AntiPatternsDemo.Services;

namespace AntiPatternsDemo.Tests
{
    // Bad: Non-descriptive test class name
    public class BadOrderServiceTests
    {
        private readonly BadOrderService _service = new();

        // Bad: Testing multiple behaviors in one test
        [Fact]
        public async Task Test1()
        {
            // Bad: Non-descriptive test name
            // Bad: Testing multiple things in one test
            // Bad: Testing implementation details
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

            // Bad: Testing too many things
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

        // Bad: Testing private implementation details
        [Fact]
        public async Task TestEmailSending()
        {
            // Bad: Testing private method
            // Bad: No proper test isolation
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

            // Bad: Testing side effects
            Assert.NotNull(order);
        }

        // Bad: No proper test cleanup
        // Bad: No proper test setup
        // Bad: No proper test teardown
    }
} 