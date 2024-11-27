using LaptopStore.Data.Contexts;
using LaptopStore.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LaptopStore.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserRepository> _logger;


        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<UserRepository> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Lấy tất cả người dùng
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users.IgnoreQueryFilters().ToListAsync();
        }

        // Lấy người dùng theo ID
        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
        }

        // Cập nhật thông tin người dùng
        public async Task<bool> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
            }
            return result.Succeeded;
        }

        // Lấy người dùng theo vai trò và tìm kiếm
        public async Task<IEnumerable<ApplicationUser>> GetUsersByRoleAsync(string roleName, string search)
        {
            var usersInRole = await (from user in _context.Users.IgnoreQueryFilters()
                                     join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                     join role in _context.Roles on userRole.RoleId equals role.Id
                                     where role.NormalizedName == roleName.ToUpper()
                                     select user).ToListAsync();

            if (!string.IsNullOrEmpty(search))
            {
                usersInRole = usersInRole.Where(u => u.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return usersInRole;
        }

        // Lấy danh sách người dùng không hoạt động kể từ một thời điểm nhất định
        public async Task<IEnumerable<ApplicationUser>> GetUsersInactiveSinceAsync(DateTime inactiveSince)
        {
            return await _context.Users.Where(u => u.LastLoginDate == null || u.LastLoginDate <= inactiveSince).ToListAsync();
        }

        // Lấy danh sách người dùng với phân trang
        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(int page, int pageSize)
        {
            return await _context.Users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // Lấy thông tin cá nhân của người dùng
        public async Task<ApplicationUser> GetProfileAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
