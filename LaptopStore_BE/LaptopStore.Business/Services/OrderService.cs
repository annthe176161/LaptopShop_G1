using LaptopStore.Business.DTOs;
using LaptopStore.Business.Interfaces;
using LaptopStore.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailSender _emailSender;

        public OrderService(IOrderRepository orderRepository, IEmailSender emailSender)
        {
            _orderRepository = orderRepository;
            _emailSender = emailSender;
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return null;

            // Kiểm tra trạng thái của người dùng
            if (!order.User.IsActive)
            {
                throw new InvalidOperationException("This user's account is currently locked.");
            }

            return new OrderDTO
            {
                OrderID = order.OrderID,
                UserID = order.UserID,
                CustomerName = order.User.Name,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                ShippingAddress = order.ShippingAddress,
                ShippingMethod = order.ShippingMethod,
                Notes = order.Notes,
                PhoneNumber = order.User.PhoneNumber,
                OrderDetails = order.OrderDetails.Select(d => new OrderDetailDTO
                {
                    ProductID = d.ProductID,
                    ProductName = d.ProductName,
                    ProductPrice = d.ProductPrice,
                    Quantity = d.Quantity,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersAsync(string status = null, string searchTerm = null, int pageNumber = 1, int pageSize = 10)
        {
            var orders = await _orderRepository.GetOrdersAsync(status, searchTerm, pageNumber, pageSize);
            return orders.Select(o => new OrderDTO
            {
                OrderID = o.OrderID,
                UserID = o.UserID,
                CustomerName = o.User.Name,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus,
                PaymentMethod = o.PaymentMethod,
                ShippingAddress = o.ShippingAddress,
                ShippingMethod = o.ShippingMethod,
                Notes = o.Notes,
                PhoneNumber = o.User.PhoneNumber,
                OrderDetails = o.OrderDetails.Select(d => new OrderDetailDTO
                {
                    ProductID = d.ProductID,
                    ProductName = d.ProductName,
                    ProductPrice = d.ProductPrice,
                    Quantity = d.Quantity,
                    Subtotal = d.Subtotal
                }).ToList()
            });
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? reason = null)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return false;

            // Cập nhật trạng thái đơn hàng
            order.OrderStatus = status;
            var updateResult = await _orderRepository.UpdateOrderAsync(order);

            // Gửi email khi trạng thái là "Rejected"
            if (status == "Rejected" && !string.IsNullOrEmpty(reason))
            {
                await SendRejectionEmailAsync(order.UserID, reason);
            }

            return updateResult;
        }

        private async Task SendRejectionEmailAsync(string userId, string reason)
        {
            // Giả sử bạn có một phương thức lấy email của người dùng
            var customerEmail = await _orderRepository.GetCustomerEmailByUserIdAsync(userId);

            if (!string.IsNullOrEmpty(customerEmail))
            {
                var subject = "Order Rejected Notification";
                var body = $@"
                <p>Dear Customer,</p>
                <p>We regret to inform you that your order has been rejected for the following reason:</p>
                <blockquote>{reason}</blockquote>
                <p>Thank you for your understanding.</p>";

                // Gửi email qua EmailSender
                await _emailSender.SendEmailAsync(customerEmail, subject, body);
            }
        }


        // 
        public async Task<List<OrderDTO>> GetOrdersWithDetailsByUserAsync(string userId)
        {
            var orders = await _orderRepository.GetOrdersWithDetailsByUserIdAsync(userId);

            return orders.Select(o => new OrderDTO
            {
                OrderID = o.OrderID,
                UserID = o.UserID,
                CustomerName = o.User?.Name ?? "Unknown",
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus,
                PaymentMethod = o.PaymentMethod,
                ShippingAddress = o.ShippingAddress,
                ShippingMethod = o.ShippingMethod,
                Notes = o.Notes,
                OrderDetails = o.OrderDetails.Select(d => new OrderDetailDTO
                {
                    ProductID = d.ProductID,
                    ProductName = d.ProductName,
                    ProductPrice = d.ProductPrice,
                    Quantity = d.Quantity,
                    Subtotal = d.Subtotal
                }).ToList()
            }).ToList();
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            return await _orderRepository.CancelOrderAsync(orderId, userId);
        }
    }
}
