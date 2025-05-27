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
        }
    }
}
