using LaptopStore.Business.DTOs;
using LaptopStore.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Security.Claims;

namespace LaptopStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // Lấy danh sách người dùng với phân trang
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedUsers(int page = 1, int pageSize = 20)
        {
            var users = await _userService.GetPagedUsersAsync(page, pageSize);
            return Ok(users);
        }

        // Thay đổi trạng thái truy cập của người dùng (isActive)
        [HttpPost("toggle-access/{userId}")]
        public async Task<IActionResult> ToggleUserAccess(string userId)
        {
            var result = await _userService.ToggleUserAccessAsync(userId);
            if (!result)
            {
                return BadRequest(new { message = "Failed to update user access." });
            }
            return Ok(new { message = "User access updated successfully." });
        }

        // Lấy danh sách khách hàng theo vai trò với tìm kiếm và phân trang
        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers(string search = "", int page = 1, int pageSize = 10, int inactivePeriod = -1)
        {
            if (inactivePeriod == -1)
            {
                // Lấy tất cả người dùng
                var customers = await _userService.GetUsersByRoleAsync("Customer", search, page, pageSize);
                return Ok(customers);
            }
            else
            {
                // Lấy người dùng không hoạt động trong một khoảng thời gian nhất định
                var inactiveCustomers = await _userService.GetInactiveUsersAsync(inactivePeriod);
                return Ok(inactiveCustomers);
            }
        }

        // Bật quyền truy cập cho người dùng
        [HttpPost("enable-access/{userId}")]
        public async Task<IActionResult> EnableAccess(string userId)
        {
            _logger.LogInformation($"Received request to enable access for user {userId}");

            var result = await _userService.EnableUserAccessAsync(userId);

            if (!result)
            {
                _logger.LogError($"Failed to enable access for user {userId}");
                return NotFound(new { message = "User not found or cannot be updated." });
            }

            _logger.LogInformation($"User {userId} access enabled successfully.");
            return Ok(new { message = "User access enabled successfully." });
        }

        // Lấy danh sách người dùng không hoạt động trong một khoảng thời gian nhất định
        [HttpGet("inactive-users")]
        public async Task<IActionResult> GetInactiveUsers([FromQuery] int daysInactive = 30)
        {
            _logger.LogInformation($"Received request to get users inactive for {daysInactive} days or more.");

            var users = await _userService.GetInactiveUsersAsync(daysInactive);
            return Ok(users);
        }

        // Gửi email mời một người dùng quay lại hệ thống
        [HttpPost("invite-user/{userId}")]
        public async Task<IActionResult> InviteUser(string userId)
        {
            _logger.LogInformation($"Received request to invite user {userId}");

            var result = await _userService.InviteUserAsync(userId);

            if (!result)
            {
                _logger.LogError($"Failed to invite user {userId}");
                return NotFound(new { message = "User not found or cannot be invited." });
            }

            return Ok(new { message = "User invited successfully." });
        }

        // Xuất danh sách người dùng theo nhóm hoặc toàn bộ
        [HttpGet("export-users")]
        public async Task<IActionResult> ExportUsers([FromQuery] int inactivePeriod = -1)
        {
            _logger.LogInformation($"Received request to export users, inactive period: {inactivePeriod}");

            IEnumerable<UserDto> users;
            if (inactivePeriod == -1)
            {
                // Lấy tất cả người dùng
                users = await _userService.GetAllUsersAsync();
            }
            else
            {
                // Lấy người dùng không hoạt động theo khoảng thời gian
                users = await _userService.GetInactiveUsersAsync(inactivePeriod);
            }

            // Tạo file Excel từ danh sách người dùng
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Users");
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "UserName";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "IsActive";
            worksheet.Cells[1, 5].Value = "LastLoginDate";
            worksheet.Cells[1, 6].Value = "Name";

            int row = 2;
            foreach (var user in users)
            {
                worksheet.Cells[row, 1].Value = user.Id;
                worksheet.Cells[row, 2].Value = user.UserName;
                worksheet.Cells[row, 3].Value = user.Email;
                worksheet.Cells[row, 4].Value = user.IsActive ? "Yes" : "No";
                worksheet.Cells[row, 5].Value = user.LastLoginDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A";
                worksheet.Cells[row, 6].Value = user.Name;
                row++;
            }

            // Điều chỉnh kích thước cột phù hợp với nội dung
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        // Lấy thông tin cá nhân của người dùng
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile(string token)
        {
            var userId = AuthenticationController.InspectTokenByClaimType(token, ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userProfile = await _userService.GetProfileAsync(userId);
            if (userProfile == null)
            {
                return NotFound("User not found");
            }

            return Ok(userProfile);
        }

        // Cập nhật số điện thoại của người dùng
        [HttpPut("update-phone")]
        public async Task<IActionResult> UpdatePhone([FromQuery] string token, [FromBody] UpdatePhoneNumberDto phoneNumberDto)
        {
            if (string.IsNullOrEmpty(phoneNumberDto.PhoneNumber))
            {
                return BadRequest(new { message = "Phone number is required." });
            }

            var userId = AuthenticationController.InspectTokenByClaimType(token, ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _userService.UpdatePhoneNumberAsync(userId, phoneNumberDto.PhoneNumber);
            if (!result)
            {
                return BadRequest(new { message = "Failed to update phone number." });
            }

            return Ok(new { message = "Phone number updated successfully." });
        }
    }
}