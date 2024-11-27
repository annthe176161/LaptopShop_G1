using LaptopStore.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaptopStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderCustomerController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderCustomerController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string ExtractUserIdFromToken(string token)
        {
            return AuthenticationController.InspectTokenByClaimType(token, ClaimTypes.NameIdentifier);
        }

        [HttpGet("user-orders")]
        public async Task<IActionResult> GetUserOrders([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token is required" });
            }

            var userId = ExtractUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid or expired token" });
            }

            var orders = await _orderService.GetOrdersWithDetailsByUserAsync(userId);
            if (orders == null || !orders.Any())
            {
                return NotFound(new { message = "No orders found" });
            }

            return Ok(orders);
        }

        [HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Token is required" });
            }

            var userId = ExtractUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid or expired token" });
            }

            var result = await _orderService.CancelOrderAsync(orderId, userId);
            if (!result)
            {
                return BadRequest(new { message = "Order cannot be canceled" });
            }

            return Ok(new { message = "Order canceled successfully" });
        }
    }
}