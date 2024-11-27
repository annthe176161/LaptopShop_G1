using Microsoft.AspNetCore.Identity;

namespace LaptopStore.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;

        // Thêm thuộc tính LastLoginDate để theo dõi thời gian đăng nhập cuối cùng
        public DateTime? LastLoginDate { get; set; }

        public string Name { get; set; }

        // Quan hệ với các bảng khác
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
