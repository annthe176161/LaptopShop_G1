using LaptopStore.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Data.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync(string status = null, string searchTerm = null, int pageNumber = 1, int pageSize = 10);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<string> GetCustomerEmailByUserIdAsync(string userId);

        // Khách hàng xem danh sách đơn hàng của họ.
        Task<List<Order>> GetOrdersWithDetailsByUserIdAsync(string userId);
        Task<bool> CancelOrderAsync(int orderId, string userId);
    }
}
