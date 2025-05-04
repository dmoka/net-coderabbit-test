using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntiPatternsDemo.Services;

namespace AntiPatternsDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _service = new();

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                if (request.Price <= 0)
                {
                    return BadRequest("Price must be greater than 0");
                }

                var order = await _service.ProcessOrder(
                    request.CustomerName,
                    request.ProductName,
                    request.Quantity,
                    request.Price,
                    request.Address,
                    request.Phone,
                    request.Email,
                    request.IsVip,
                    request.PaymentMethod,
                    request.CardNumber
                );

                if (order == null)
                {
                    return StatusCode(500, "Something went wrong");
                }

                return Ok(new
                {
                    orderId = order.Id,
                    customer = order.CustomerName,
                    product = order.ProductName,
                    qty = order.Quantity,
                    price = order.Price,
                    total = order.Total,
                    status = order.Status,
                    created = order.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class CreateOrderRequest
    {
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsVip { get; set; }
        public string PaymentMethod { get; set; }
        public string CardNumber { get; set; }
    }
} 