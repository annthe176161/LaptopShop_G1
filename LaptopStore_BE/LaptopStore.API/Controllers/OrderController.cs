using LaptopStore.Data.Contexts;
using LaptopStore.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LaptopStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrderAsync(
                    [FromBody] List<CartItem> cartItems,
                    string shippingAddress,
                    string paymentMethod,
                    string shippingMethod,
                    string notes,
                    string email)
        {

          
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                decimal totalAmount = 0;
                var orderDetails = new List<OrderDetail>();
                var productsToUpdate = new List<Product>();

                var order = new Order
                {
                    UserID = user.Id,
                    OrderDate = DateTime.UtcNow,
                    PaymentStatus = "Pending",
                    ShippingAddress = shippingAddress,
                    PaymentMethod = paymentMethod,
                    ShippingMethod = shippingMethod,
                    TrackingNumber = "VL",
                    Notes = notes,
                    TotalAmount = 0
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                foreach (var cartItem in cartItems)
                {
                    var product = await _context.Products
                                                .FirstOrDefaultAsync(p => p.ProductID == cartItem.ProductID && !p.IsDeleted);

                    if (product == null)
                    {
                        return BadRequest($"Product with ID {cartItem.ProductID} is unavailable or deleted.");
                    }

                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        return BadRequest($"Product '{product.Name}' has insufficient stock.");
                    }

                    // Update total amount
                    totalAmount += product.Price * cartItem.Quantity;

                    // Create order detail
                    var orderDetail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = product.ProductID,
                        Quantity = cartItem.Quantity,
                        ProductName = product.Name,
                        ProductPrice = product.Price,
                        Subtotal = product.Price * cartItem.Quantity
                    };

                    product.StockQuantity -= cartItem.Quantity;

                    await _context.OrderDetails.AddAsync(orderDetail);
                    _context.Products.Update(product);
                }
                order.TotalAmount = totalAmount;
                _context.Orders.Update(order);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return Ok(new
                    {
                        Message = "Order placed successfully.",
                        OrderId = order.OrderID,
                        TotalAmount = order.TotalAmount
                    });
                }

                return BadRequest("Failed to place the order. Please try again.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error placing order: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while placing the order.");
            }
        }
    }
}
