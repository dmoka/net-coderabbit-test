using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace AntiPatternsDemo.Services
{
    public class OrderService
    {
        private readonly string _connString = "Server=localhost;Database=shop;User=admin;Password=123456";
        private static List<Order> _orders = new();

        public async Task<Order> ProcessOrder(string customerName, string productName, int qty, decimal price, string address, string phone, string email, bool isVip, string paymentMethod, string cardNumber)
        {
            if (string.IsNullOrEmpty(customerName)) return null;
            if (string.IsNullOrEmpty(productName)) return null;
            if (qty <= 0) return null;
            if (price <= 0) return null;

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

            using (var conn = new SqlConnection(_connString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand($"INSERT INTO Orders VALUES ('{order.Id}', '{order.CustomerName}', '{order.ProductName}', {order.Quantity}, {order.Price}, {order.Total}, '{order.Status}', '{order.CreatedAt}')", conn);
                await cmd.ExecuteNonQueryAsync();
            }

            await SendEmail(email, "Order Confirmation", $"Your order for {qty} {productName} has been received.");

            return order;
        }

        private async Task SendEmail(string to, string subject, string body)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential("shop@gmail.com", "password123");
                await client.SendMailAsync(new MailMessage("shop@gmail.com", to, subject, body));
            }
        }
    }

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