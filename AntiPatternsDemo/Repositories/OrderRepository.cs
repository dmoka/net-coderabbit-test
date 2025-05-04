using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using AntiPatternsDemo.Services;

namespace AntiPatternsDemo.Repositories
{
    public class OrderRepository
    {
        private readonly string _connString = "Server=localhost;Database=shop;User=admin;Password=123456";

        public async Task<List<Order>> GetOrdersByCustomer(string customerName)
        {
            var orders = new List<Order>();
            using (var conn = new SqlConnection(_connString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand($"SELECT * FROM Orders WHERE CustomerName = '{customerName}'", conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        orders.Add(new Order
                        {
                            Id = reader.GetGuid(0),
                            CustomerName = reader.GetString(1),
                            ProductName = reader.GetString(2),
                            Quantity = reader.GetInt32(3),
                            Price = reader.GetDecimal(4),
                            Total = reader.GetDecimal(5),
                            Status = reader.GetString(6),
                            CreatedAt = reader.GetDateTime(7)
                        });
                    }
                }
            }
            return orders;
        }

        public async Task SaveOrder(Order order)
        {
            using (var conn = new SqlConnection(_connString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand($"INSERT INTO Orders VALUES ('{order.Id}', '{order.CustomerName}', '{order.ProductName}', {order.Quantity}, {order.Price}, {order.Total}, '{order.Status}', '{order.CreatedAt}')", conn);
                await cmd.ExecuteNonQueryAsync();

                if (order.Total > 1000)
                {
                    await SendHighValueOrderNotification(order);
                }
            }
        }

        private async Task SendHighValueOrderNotification(Order order)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential("shop@gmail.com", "password123");
                await client.SendMailAsync(new MailMessage(
                    "shop@gmail.com",
                    "manager@shop.com",
                    "High Value Order Alert",
                    $"Order {order.Id} has a total value of {order.Total}"
                ));
            }
        }
    }
} 