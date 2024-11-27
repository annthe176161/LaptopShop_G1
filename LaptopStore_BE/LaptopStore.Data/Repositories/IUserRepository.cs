using LaptopStore.Data.Entities;

namespace LaptopStore.Data.Repositories
{
    public interface IUserRepository
    {
        // Lấy tất cả người dùng
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        // Lấy người dùng theo ID
        Task<ApplicationUser> GetUserByIdAsync(string id);
        // Cập nhật thông tin người dùng
        Task<bool> UpdateUserAsync(ApplicationUser user);
        // Lấy người dùng theo vai trò và tìm kiếm
        Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string roleName, string search);
        // Lấy danh sách người dùng không hoạt động kể từ một thời điểm nhất định
        Task<IEnumerable<ApplicationUser>> GetUsersInactiveSinceAsync(DateTime inactiveSince);
        // Lấy danh sách người dùng với phân trang
        Task<IEnumerable<ApplicationUser>> GetUsersAsync(int page, int pageSize);
        // Lấy thông tin cá nhân của người dùng
        Task<ApplicationUser> GetProfileAsync(string userId);
    }
}
