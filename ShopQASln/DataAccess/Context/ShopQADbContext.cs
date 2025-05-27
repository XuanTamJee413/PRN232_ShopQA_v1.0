using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class ShopQADbContext : DbContext
    {
        public ShopQADbContext(DbContextOptions<ShopQADbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Address> Addresses => Set<Address>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            // Category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();
            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Product
            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductVariant
            modelBuilder.Entity<ProductVariant>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProductVariant>()
                .HasIndex(v => new { v.ProductId, v.Size, v.Color })
                .IsUnique(); // đảm bảo 1 sản phẩm không trùng size + màu

            // Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(i => i.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OrderItem>()
                .HasOne(i => i.ProductVariant)
                .WithMany()
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Address
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // Cart
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .Property(c => c.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.ProductVariant)
                .WithMany()
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Method)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20);

            // Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .Property(r => r.Rating)
                .IsRequired();

            modelBuilder.Entity<Review>()
                .Property(r => r.Comment)
                .HasMaxLength(500);

            // Inventory
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.ProductVariant)
                .WithMany()
                .HasForeignKey(i => i.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>()
                .Property(i => i.Quantity)
                .IsRequired();

            // Discount
            modelBuilder.Entity<Discount>()
                .HasIndex(d => d.Code)
                .IsUnique();

            modelBuilder.Entity<Discount>()
                .Property(d => d.Code)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Discount>()
                .Property(d => d.Amount)
                .IsRequired();

            modelBuilder.Entity<Discount>()
                .Property(d => d.IsPercentage)
                .IsRequired();
            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            // Category
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Áo sơ mi" },
                new Category { Id = 2, Name = "Quần tây" },
                new Category { Id = 3, Name = "Đầm nữ" }
            );

            // User
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "nguyenvana", Email = "vana@gmail.com", PasswordHash = "hash1", Role = "Customer" },
                new User { Id = 2, Username = "admin", Email = "admin@shopqa.vn", PasswordHash = "adminhash", Role = "Admin" }
            );

            // Address
            modelBuilder.Entity<Address>().HasData(
                new Address { Id = 1, UserId = 1, Street = "123 Nguyễn Trãi", City = "Hà Nội", Country = "Việt Nam" }
            );

            // Product
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Áo sơ mi trắng nam",
                    Description = "Form Hàn Quốc, vải lụa mát, thích hợp đi học và đi làm.",
                    Price = 350000,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Quần tây công sở",
                    Description = "Vải co giãn, mặc nhẹ thoáng mát.",
                    Price = 420000,
                    CategoryId = 2
                }
            );

            // ProductVariant
            modelBuilder.Entity<ProductVariant>().HasData(
                new ProductVariant { Id = 1, ProductId = 1, Size = "M", Color = "Trắng", Stock = 20 },
                new ProductVariant { Id = 2, ProductId = 1, Size = "L", Color = "Xanh nhạt", Stock = 10 },
                new ProductVariant { Id = 3, ProductId = 2, Size = "32", Color = "Đen", Stock = 15 }
            );

            // Order
            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, UserId = 1, OrderDate = DateTime.Parse("2025-05-20"), TotalAmount = 770000 }
            );

            // OrderItem
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { Id = 1, OrderId = 1, ProductVariantId = 1, Quantity = 1, Price = 350000 },
                new OrderItem { Id = 2, OrderId = 1, ProductVariantId = 3, Quantity = 1, Price = 420000 }
            );

            // Cart
            modelBuilder.Entity<Cart>().HasData(
                new Cart { Id = 1, UserId = 1, CreatedAt = DateTime.Parse("2025-05-21") }
            );
            modelBuilder.Entity<CartItem>().HasData(
                new CartItem { Id = 1, CartId = 1, ProductVariantId = 1, Quantity = 2 }
            );

            // Payment
            modelBuilder.Entity<Payment>().HasData(
                new Payment { Id = 1, OrderId = 1, Method = "COD", Amount = 770000, PaidAt = DateTime.Parse("2025-05-20"), Status = "Completed" }
            );

            // Review
            modelBuilder.Entity<Review>().HasData(
                new Review { Id = 1, UserId = 1, ProductId = 1, Rating = 5, Comment = "Sản phẩm rất tốt!", CreatedAt = DateTime.Parse("2025-05-22") }
            );

            // Inventory
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { Id = 1, ProductVariantId = 1, Quantity = 20, UpdatedAt = DateTime.Parse("2025-05-20") }
            );

            // Discount
            modelBuilder.Entity<Discount>().HasData(
                new Discount { Id = 1, Code = "SALE10", Amount = 10, IsPercentage = true, StartDate = DateTime.Parse("2025-05-01"), EndDate = DateTime.Parse("2025-06-01"), UsageLimit = 100 }
            );

        }
    }
}
