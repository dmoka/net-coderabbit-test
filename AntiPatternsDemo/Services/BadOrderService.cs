using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntiPatternsDemo.Services
{
    // Bad: Service does too many things (violates SRP)
    public class BadOrderService
    {
        private readonly string _connString = "Server=localhost;Database=shop;User=admin;Password=123456"; // Bad: Hardcoded credentials
        private static List<Order> _orders = new(); // Bad: Static field, potential memory leak

        // Bad: Method does too many things
        public async Task<Order> ProcessOrder(string customerName, string productName, int qty, decimal price, string address, string phone, string email, bool isVip, string paymentMethod, string cardNumber)
        {
            // Bad: Too many parameters
            if (string.IsNullOrEmpty(customerName)) return null; // Bad: Using null instead of Result pattern
            if (string.IsNullOrEmpty(productName)) return null;
            if (qty <= 0) return null;
            if (price <= 0) return null;

            // Bad: Business logic mixed with data access
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = customerName,
                ProductName = productName,
                Quantity = qty,
                Price = price,
                Total = qty * price,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            // Bad: Direct database access in service
            using (var conn = new SqlConnection(_connString))
            {
                await conn.OpenAsync();
                // Bad: Raw SQL instead of ORM
                var cmd = new SqlCommand($"INSERT INTO Orders VALUES ('{order.Id}', '{order.CustomerName}', '{order.ProductName}', {order.Quantity}, {order.Price}, {order.Total}, '{order.Status}', '{order.CreatedAt}')", conn);
                await cmd.ExecuteNonQueryAsync();
            }

            // Bad: Mixed responsibilities - sending email
            await SendEmail(email, "Order Confirmation", $"Your order for {qty} {productName} has been received.");

            return order;
        }

        // Bad: Private method that should be in a separate service
        private async Task SendEmail(string to, string subject, string body)
        {
            // Bad: Implementation details
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential("shop@gmail.com", "password123");
                await client.SendMailAsync(new MailMessage("shop@gmail.com", to, subject, body));
            }
        }
    }

    // Bad: Class with too many responsibilities
    public class Order
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 