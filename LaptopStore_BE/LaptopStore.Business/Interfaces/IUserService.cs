using LaptopStore.Business.DTOs;

namespace LaptopStore.Business.Interfaces
{
    public interface IUserService
    {
        // Lấy danh sách người dùng với phân trang
        Task<IEnumerable<UserDto>> GetPagedUsersAsync(int page, int pageSize);
        // Thay đổi trạng thái truy cập (isActive)
        Task<bool> ToggleUserAccessAsync(string userId);
        // Bật quyền truy cập cho người dùng (Enable Access)
        Task<bool> EnableUserAccessAsync(string userId);
        // Lấy người dùng theo vai trò và tìm kiếm
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName, string search, int page, int pageSize);
        // Lấy danh sách người dùng không hoạt động trong một khoảng thời gian nhất định
        Task<IEnumerable<UserDto>> GetInactiveUsersAsync(int daysInactive);
        // Gửi email mời một người dùng quay lại hệ thống
        Task<bool> InviteUserAsync(string userId);
        // Lấy tất cả người dùng
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        // Lấy thông tin cá nhân của người dùng
        Task<UserProfileDto> GetProfileAsync(string userId);
        // Cập nhật số điện thoại của người dùng
        Task<bool> UpdatePhoneNumberAsync(string userId, string phoneNumber);

    }
}
