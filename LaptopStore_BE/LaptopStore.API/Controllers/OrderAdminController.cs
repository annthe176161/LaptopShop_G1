using LaptopStore.Business.DTOs;
using LaptopStore.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LaptopStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAdminController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderAdminController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrdersAsync(
            [FromQuery] string status = null,
            [FromQuery] string searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetOrdersAsync(status, searchTerm, pageNumber, pageSize);
            return Ok(orders);
        }


        [HttpGet("GetOrderById/{orderId}")]
        public async Task<IActionResult> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null) return NotFound("Order not found.");

                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("UpdateOrderStatus/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatusAsync(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            if (string.IsNullOrEmpty(request.Status))
            {
                return BadRequest(new { success = false, message = "Invalid status data." });
            }

            var result = await _orderService.UpdateOrderStatusAsync(orderId, request.Status, request.Reason);

            if (!result)
            {
                return BadRequest(new { success = false, message = "Failed to update order status." });
            }

            return Ok(new { success = true, message = "Order status updated successfully." });
        }
    }
}
