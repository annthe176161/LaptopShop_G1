using LaptopStore.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LaptopStore.Data.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet cho các bảng
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Gọi base method để xử lý Identity

            // Cấu hình khóa ngoại và hành vi xóa

            // OrderDetail - Order
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderDetail - Product
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductSpecification - Product
            builder.Entity<ProductSpecification>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSpecifications)
                .HasForeignKey(ps => ps.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductSpecification - ProductAttribute
            builder.Entity<ProductSpecification>()
                .HasOne(ps => ps.ProductAttribute)
                .WithMany(pa => pa.ProductSpecifications)
                .HasForeignKey(ps => ps.AttributeID)
                .OnDelete(DeleteBehavior.Restrict);

            // Review - Product
            builder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            // ShoppingCart - Product
            builder.Entity<ShoppingCart>()
                .HasOne(sc => sc.Product)
                .WithMany(p => p.ShoppingCarts)
                .HasForeignKey(sc => sc.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - Discount
            builder.Entity<Product>()
                .HasOne(p => p.Discount)
                .WithMany(d => d.Products)
                .HasForeignKey(p => p.DiscountID)
                .OnDelete(DeleteBehavior.SetNull);

            // Order - ApplicationUser
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // Review - ApplicationUser
            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // ShoppingCart - ApplicationUser
            builder.Entity<ShoppingCart>()
                .HasOne(sc => sc.User)
                .WithMany(u => u.ShoppingCarts)
                .HasForeignKey(sc => sc.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // RefreshToken - ApplicationUser
            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Brand - Product
            builder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandID)
                .OnDelete(DeleteBehavior.Restrict);

            // Global Query Filters cho soft delete
            builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Brand>().HasQueryFilter(b => !b.IsDeleted);
            builder.Entity<Discount>().HasQueryFilter(d => !d.IsDeleted);
            builder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);
            builder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
            builder.Entity<ApplicationUser>().HasQueryFilter(u => u.IsActive);
        }
    }
}
