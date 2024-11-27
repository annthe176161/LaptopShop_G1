using LaptopStore.Business.DTOs;
using LaptopStore.Business.Interfaces;
using LaptopStore.Data.Entities;
using LaptopStore.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;

namespace LaptopStore.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        // Cập nhật constructor để nhận _emailService từ DI
        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _logger = logger;
            _emailService = emailService;
            _userManager = userManager;
        }

        // Gửi email mời một người dùng quay lại hệ thống
        public async Task<bool> InviteUserAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"User {userId} not found.");
                return false;
            }

            string subject = "We Miss You at Our Platform!";
            string message = $"Hello {user.Name},\n\nWe've noticed you haven't been active for a while. Come back and see what's new!";

            await _emailService.SendAsync(user.Email, subject, message, isHtml: false);
            _logger.LogInformation($"Invitation email sent to user {userId}.");

            return true;
        }

        // Lấy tất cả người dùng
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                IsActive = u.IsActive,
                LastLoginDate = u.LastLoginDate,
                Name = u.Name
            }).ToList();
        }

        // Lấy danh sách người dùng với phân trang
        public async Task<IEnumerable<UserDto>> GetPagedUsersAsync(int page, int pageSize)
        {
            var users = await _userRepository.GetUsersAsync(page, pageSize);
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                IsActive = u.IsActive,
                LastLoginDate = u.LastLoginDate,
                Name = u.Name
            }).ToList();
        }

        // Thay đổi trạng thái truy cập (isActive)
        public async Task<bool> ToggleUserAccessAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = !user.IsActive;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }

        // Bật quyền truy cập cho người dùng
        public async Task<bool> EnableUserAccessAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = true;
            await _userRepository.UpdateUserAsync(user);
            return true;
        }

        // Lấy người dùng theo vai trò và tìm kiếm
        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName, string search, int page, int pageSize)
        {
            var users = await _userRepository.GetUsersByRoleAsync(roleName, search);
            return users.Skip((page - 1) * pageSize).Take(pageSize).Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                IsActive = u.IsActive,
                LastLoginDate = u.LastLoginDate,
                Name = u.Name
            }).ToList();
        }

        // Lấy danh sách người dùng không hoạt động trong một khoảng thời gian nhất định
        public async Task<IEnumerable<UserDto>> GetInactiveUsersAsync(int daysInactive)
        {
            DateTime inactiveSince = DateTime.Now.AddDays(-daysInactive);
            var users = await _userRepository.GetUsersInactiveSinceAsync(inactiveSince);
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                IsActive = u.IsActive,
                LastLoginDate = u.LastLoginDate,
                Name = u.Name
            }).ToList();
        }

        // Lấy thông tin cá nhân của người dùng
        public async Task<UserProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            // Lấy role của người dùng
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return new UserProfileDto
            {
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = role
            };
        }

        // Cập nhật số điện thoại của người dùng
        public async Task<bool> UpdatePhoneNumberAsync(string userId, string phoneNumber)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.PhoneNumber = phoneNumber; // Cập nhật số điện thoại
            return await _userRepository.UpdateUserAsync(user);
        }
    }
}
