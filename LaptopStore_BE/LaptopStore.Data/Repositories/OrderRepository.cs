using LaptopStore.Data.Contexts;
using LaptopStore.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopStore.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.IsDeleted = true;
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<string> GetCustomerEmailByUserIdAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user?.Email;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order != null)
            {
                // Load the User explicitly, ignoring query filters
                await _context.Entry(order)
                    .Reference(o => o.User)
                    .Query()
                    .IgnoreQueryFilters()
                    .LoadAsync();
            }

            return order;
        }

        // LaptopStore.Data.Repositories.OrderRepository
        public async Task<IEnumerable<Order>> GetOrdersAsync(string status = null, string searchTerm = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.User)
                .AsQueryable();

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }

            // Tìm kiếm theo tên khách hàng
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(o => o.User.Name.Contains(searchTerm));
            }

            // Áp dụng phân trang
            query = query.OrderBy(o => o.OrderDate)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }





        // Khách hàng xem danh sách đơn hàng của họ.
        public async Task<List<Order>> GetOrdersWithDetailsByUserIdAsync(string userId)
        {
            return await _context.Orders
        .Where(o => o.UserID == userId && !o.IsDeleted)
        .Include(o => o.User) // Include đối tượng User
        .Include(o => o.OrderDetails) // Include OrderDetails
        .ToListAsync();
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails) // Include chi tiết đơn hàng
                .FirstOrDefaultAsync(o => o.OrderID == orderId && o.UserID == userId);

            if (order == null || (order.OrderStatus != "Pending" && order.OrderStatus != "Confirmed"))
                return false;

            // Trả lại số lượng sản phẩm vào kho
            foreach (var detail in order.OrderDetails)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == detail.ProductID);
                if (product != null)
                {
                    product.StockQuantity += detail.Quantity;
                }
            }

            // Cập nhật trạng thái đơn hàng
            order.OrderStatus = "Canceled";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
