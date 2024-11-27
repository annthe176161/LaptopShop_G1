using LaptopStore.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Business.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetOrdersAsync(string status = null, string searchTerm = null, int pageNumber = 1, int pageSize = 10);
        Task<OrderDTO> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? reason = null);

        // Khách hàng xem danh sách đơn hàng của họ.
        Task<List<OrderDTO>> GetOrdersWithDetailsByUserAsync(string userId);
        Task<bool> CancelOrderAsync(int orderId, string userId);
    }
}
