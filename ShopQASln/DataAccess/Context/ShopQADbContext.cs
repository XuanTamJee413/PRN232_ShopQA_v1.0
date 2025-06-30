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
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Cart> Cart => Set<Cart>();
        public DbSet<Review> Review { get; set; } = default!;
        public DbSet<Inventory> Inventory => Set<Inventory>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

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

            // Brand
            modelBuilder.Entity<Brand>()
                .Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Brand>()
                .HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

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
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductVariant
            modelBuilder.Entity<ProductVariant>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

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
       .WithOne(pv => pv.Inventory)
       .HasForeignKey<Inventory>(i => i.ProductVariantId)
       .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Inventory>()
                .Property(i => i.Quantity)
                .IsRequired();

            // Discount
            modelBuilder.Entity<Discount>()
                .Property(d => d.Amount)
                .IsRequired();
            // Wishlist
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Wishlist>()
                .Property(w => w.CreatedAt)
                .IsRequired();
            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Wishlist)
                .WithMany(w => w.Items)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<WishlistItem>()
                .HasOne(wi => wi.Product)
                .WithMany()
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WishlistItem>()
                .Property(wi => wi.AddedAt)
                .IsRequired();
            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            // Category
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Áo thun" },
                new Category { Id = 2, Name = "Áo khoác" },
                new Category { Id = 3, Name = "Chân váy" },
                new Category { Id = 4, Name = "Quần jean" },
                new Category { Id = 5, Name = "Đồ thể thao" },
                new Category { Id = 6, Name = "Đồ ngủ" },
                new Category { Id = 7, Name = "Đồ công sở" },
                new Category { Id = 8, Name = "Giày dép" },
                new Category { Id = 9, Name = "Túi xách" },
                new Category { Id = 10, Name = "Phụ kiện thời trang" }
            );

            // User
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "tranthib", Email = "tranb@gmail.com", PasswordHash = "123", Role = "Customer" },
                new User { Id = 2, Username = "leminhc", Email = "minhc@yahoo.com", PasswordHash = "123", Role = "Customer" },
                new User { Id = 3, Username = "staff", Email = "staff@shopqa.vn", PasswordHash = "123", Role = "Staff" },
                new User { Id = 4, Username = "phamthanh", Email = "thanhp@gmail.com", PasswordHash = "123", Role = "Customer" },
                new User { Id = 5, Username = "nguyenhoa", Email = "hoa.nguyen@gmail.com", PasswordHash = "123", Role = "Customer" },
                new User { Id = 6, Username = "admin", Email = "admin@shopqa.vn", PasswordHash = "123", Role = "Admin" }
            );

            // Address
            modelBuilder.Entity<Address>().HasData(
                new Address { Id = 1, UserId = 1, Street = "123 Nguyễn Trãi", City = "Hà Nội", Country = "Việt Nam" },
                new Address { Id = 2, UserId = 1, Street = "25 Điện Biên Phủ", City = "Hải Phòng", Country = "Việt Nam" },

                new Address { Id = 3, UserId = 2, Street = "56 Nguyễn Văn Linh", City = "Đà Nẵng", Country = "Việt Nam" },

                new Address { Id = 4, UserId = 4, Street = "78 Trần Hưng Đạo", City = "Hồ Chí Minh", Country = "Việt Nam" },
                new Address { Id = 5, UserId = 4, Street = "135 Nguyễn Văn Cừ", City = "Hà Nội", Country = "Việt Nam" },

                new Address { Id = 6, UserId = 5, Street = "89 Phan Đình Phùng", City = "Cần Thơ", Country = "Việt Nam" },
                new Address { Id = 7, UserId = 5, Street = "199 Cách Mạng Tháng Tám", City = "Bình Dương", Country = "Việt Nam" }
            );


            // Brand
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Việt Tiến" },
                new Brand { Id = 2, Name = "An Phước" },
                new Brand { Id = 3, Name = "May 10" },
                new Brand { Id = 4, Name = "Blue Exchange" },
                new Brand { Id = 5, Name = "Routine" },
                new Brand { Id = 6, Name = "Ninomaxx" }
            );


            // Product 
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Áo sơ mi trắng nam", Description = "Form Hàn Quốc, vải lụa mát, thích hợp đi học và đi làm.", CategoryId = 1, BrandId = 1, ImageUrl = "/images/products/ao-so-mi-trang.jpg" },
                new Product { Id = 2, Name = "Quần tây công sở", Description = "Vải co giãn, mặc nhẹ thoáng mát.", CategoryId = 2, BrandId = 2, ImageUrl = "/images/products/quan-tay.jpg" },
                new Product { Id = 3, Name = "Đầm nữ công sở", Description = "Đầm nữ dáng xòe, chất liệu cao cấp.", CategoryId = 3, BrandId = 3, ImageUrl = "/images/products/dam-nu.jpg" },
                new Product { Id = 4, Name = "Áo sơ mi caro nam", Description = "Áo sơ mi caro trẻ trung, cá tính.", CategoryId = 1, BrandId = 4, ImageUrl = "/images/products/ao-so-mi-caro.jpg" },
                new Product { Id = 5, Name = "Quần tây ống đứng", Description = "Phong cách thanh lịch, hợp thời trang công sở.", CategoryId = 2, BrandId = 5, ImageUrl = "/images/products/quan-tay-ong-dung.jpg" },
                new Product { Id = 6, Name = "Đầm suông tay lỡ", Description = "Chất cotton thoáng mát, kiểu dáng đơn giản.", CategoryId = 3, BrandId = 6, ImageUrl = "/images/products/dam-suong.jpg" },
                new Product { Id = 7, Name = "Áo sơ mi xanh dương", Description = "Vải cotton, ít nhăn, dễ ủi, thoải mái vận động.", CategoryId = 1, BrandId = 5, ImageUrl = "/images/products/ao-so-mi-xanh.jpg" },
                new Product { Id = 8, Name = "Quần tây lưng cao nữ", Description = "Thiết kế giúp tôn dáng, chất vải cao cấp.", CategoryId = 2, BrandId = 3, ImageUrl = "/images/products/quan-tay-nu.jpg" },
                new Product { Id = 9, Name = "Đầm dự tiệc ren hoa", Description = "Dáng ôm body, phong cách sang trọng.", CategoryId = 3, BrandId = 2, ImageUrl = "/images/products/dam-ren.jpg" },
                new Product { Id = 10, Name = "Áo sơ mi cổ trụ nam", Description = "Phong cách trẻ trung, lịch sự, dễ phối đồ.", CategoryId = 1, BrandId = 6, ImageUrl = "/images/products/ao-co-tru.jpg" },
                new Product { Id = 11, Name = "Quần tây slimfit nam", Description = "Tôn dáng, chất liệu mềm mại, co giãn tốt.", CategoryId = 2, BrandId = 1, ImageUrl = "/images/products/quan-slimfit.jpg" },
                new Product { Id = 12, Name = "Đầm body tay dài ren", Description = "Dáng ôm body, tôn dáng, phù hợp dự tiệc.", CategoryId = 3, BrandId = 4, ImageUrl = "/images/products/dam-body.jpg" },
                new Product { Id = 13, Name = "Áo sơ mi lụa nữ cao cấp", Description = "Vải lụa cao cấp, mềm mại, không nhăn.", CategoryId = 1, BrandId = 5, ImageUrl = "/images/products/ao-lua-nu.jpg" },
                new Product { Id = 14, Name = "Quần tây nam màu xám", Description = "Màu xám, đơn giản, thích hợp đi làm.", CategoryId = 2, BrandId = 2, ImageUrl = "/images/products/quan-basic.jpg" },
                new Product { Id = 15, Name = "Đầm hoa nhí xòe nhẹ", Description = "Phong cách dễ thương, nhẹ nhàng.", CategoryId = 3, BrandId = 6, ImageUrl = "/images/products/dam-hoa-nhi.jpg" },
                new Product { Id = 16, Name = "Áo sơ mi linen nam trắng", Description = "Thoáng mát, phù hợp mùa hè.", CategoryId = 1, BrandId = 3, ImageUrl = "/images/products/ao-linen.jpg" },
                new Product { Id = 17, Name = "Quần kaki công sở", Description = "Màu nâu, chất kaki, co giãn nhẹ.", CategoryId = 2, BrandId = 4, ImageUrl = "/images/products/quan-kaki.jpg" },
                new Product { Id = 18, Name = "Đầm maxi tay bồng trắng", Description = "Phong cách công chúa, phù hợp dạo phố.", CategoryId = 3, BrandId = 5, ImageUrl = "/images/products/dam-maxi.jpg" },
                new Product { Id = 19, Name = "Áo sơ mi sọc caro nữ form rộng", Description = "Form rộng, thoải mái, trẻ trung.", CategoryId = 1, BrandId = 2, ImageUrl = "/images/products/ao-caro-nu.jpg" },
                new Product { Id = 20, Name = "Quần lửng nữ mùa hè", Description = "Thiết kế thời trang, mát mẻ cho mùa hè.", CategoryId = 2, BrandId = 1, ImageUrl = "/images/products/quan-lung.jpg" }
            );



            // ProductVariant
            modelBuilder.Entity<ProductVariant>().HasData(
                // Product 1
                new ProductVariant { Id = 1, ProductId = 1, Size = "M", Color = "Trắng", Stock = 20, Price = 350000, ImageUrl = "/images/products/ao-so-mi-trang-m.jpg" },
                new ProductVariant { Id = 2, ProductId = 1, Size = "L", Color = "Trắng", Stock = 15, Price = 355000, ImageUrl = "/images/products/ao-so-mi-trang-l.jpg" },
                new ProductVariant { Id = 3, ProductId = 1, Size = "XL", Color = "Xanh", Stock = 10, Price = 360000, ImageUrl = "/images/products/ao-so-mi-trang-xl.jpg" },
                new ProductVariant { Id = 4, ProductId = 1, Size = "M", Color = "Đỏ", Stock = 12, Price = 355000, ImageUrl = "/images/products/ao-so-mi-trang-m-do.jpg" },

                // Product 2
                new ProductVariant { Id = 5, ProductId = 2, Size = "32", Color = "Đen", Stock = 10, Price = 420000, ImageUrl = "/images/products/quan-tay-den-32.jpg" },
                new ProductVariant { Id = 6, ProductId = 2, Size = "34", Color = "Xám", Stock = 15, Price = 430000, ImageUrl = "/images/products/quan-tay-xam-34.jpg" },
                new ProductVariant { Id = 7, ProductId = 2, Size = "36", Color = "Đen", Stock = 8, Price = 435000, ImageUrl = "/images/products/quan-tay-den-36.jpg" },

                // Product 3
                new ProductVariant { Id = 8, ProductId = 3, Size = "S", Color = "Đỏ", Stock = 8, Price = 500000, ImageUrl = "/images/products/dam-nu-do-s.jpg" },
                new ProductVariant { Id = 9, ProductId = 3, Size = "M", Color = "Đen", Stock = 5, Price = 520000, ImageUrl = "/images/products/dam-nu-den-m.jpg" },
                new ProductVariant { Id = 10, ProductId = 3, Size = "L", Color = "Trắng", Stock = 3, Price = 530000, ImageUrl = "/images/products/dam-nu-trang-l.jpg" },

                // Product 4
                new ProductVariant { Id = 11, ProductId = 4, Size = "M", Color = "Caro Xanh", Stock = 20, Price = 370000, ImageUrl = "/images/products/ao-so-mi-caro-xanh-m.jpg" },
                new ProductVariant { Id = 12, ProductId = 4, Size = "L", Color = "Caro Đỏ", Stock = 15, Price = 375000, ImageUrl = "/images/products/ao-so-mi-caro-do-l.jpg" },
                new ProductVariant { Id = 13, ProductId = 4, Size = "XL", Color = "Caro Đen", Stock = 10, Price = 380000, ImageUrl = "/images/products/ao-so-mi-caro-den-xl.jpg" },

                // Product 5
                new ProductVariant { Id = 14, ProductId = 5, Size = "30", Color = "Xám", Stock = 18, Price = 440000, ImageUrl = "/images/products/quan-tay-xam-30.jpg" },
                new ProductVariant { Id = 15, ProductId = 5, Size = "32", Color = "Đen", Stock = 20, Price = 445000, ImageUrl = "/images/products/quan-tay-den-32.jpg" },
                new ProductVariant { Id = 16, ProductId = 5, Size = "34", Color = "Xám", Stock = 12, Price = 450000, ImageUrl = "/images/products/quan-tay-xam-34.jpg" },

                // Product 6
                new ProductVariant { Id = 17, ProductId = 6, Size = "M", Color = "Be", Stock = 14, Price = 460000, ImageUrl = "/images/products/dam-suong-be-m.jpg" },
                new ProductVariant { Id = 18, ProductId = 6, Size = "L", Color = "Trắng", Stock = 16, Price = 470000, ImageUrl = "/images/products/dam-suong-trang-l.jpg" },
                new ProductVariant { Id = 19, ProductId = 6, Size = "XL", Color = "Xanh", Stock = 10, Price = 480000, ImageUrl = "/images/products/dam-suong-xanh-xl.jpg" },

                // Product 7
                new ProductVariant { Id = 20, ProductId = 7, Size = "M", Color = "Xanh Dương", Stock = 25, Price = 355000, ImageUrl = "/images/products/ao-so-mi-xanh-m.jpg" },
                new ProductVariant { Id = 21, ProductId = 7, Size = "L", Color = "Xanh Dương", Stock = 20, Price = 360000, ImageUrl = "/images/products/ao-so-mi-xanh-l.jpg" },
                new ProductVariant { Id = 22, ProductId = 7, Size = "XL", Color = "Xanh Dương", Stock = 10, Price = 370000, ImageUrl = "/images/products/ao-so-mi-xanh-xl.jpg" },

                // Product 8
                new ProductVariant { Id = 23, ProductId = 8, Size = "S", Color = "Đen", Stock = 10, Price = 480000, ImageUrl = "/images/products/quan-tay-nu-den-s.jpg" },
                new ProductVariant { Id = 24, ProductId = 8, Size = "M", Color = "Xám", Stock = 15, Price = 490000, ImageUrl = "/images/products/quan-tay-nu-xam-m.jpg" },
                new ProductVariant { Id = 25, ProductId = 8, Size = "L", Color = "Đỏ", Stock = 12, Price = 495000, ImageUrl = "/images/products/quan-tay-nu-do-l.jpg" },

                // Product 9
                new ProductVariant { Id = 26, ProductId = 9, Size = "M", Color = "Đỏ", Stock = 8, Price = 520000, ImageUrl = "/images/products/dam-ren-do-m.jpg" },
                new ProductVariant { Id = 27, ProductId = 9, Size = "L", Color = "Đỏ", Stock = 5, Price = 530000, ImageUrl = "/images/products/dam-ren-do-l.jpg" },
                new ProductVariant { Id = 28, ProductId = 9, Size = "XL", Color = "Đỏ", Stock = 3, Price = 540000, ImageUrl = "/images/products/dam-ren-do-xl.jpg" },

                // Product 10
                new ProductVariant { Id = 29, ProductId = 10, Size = "M", Color = "Trắng", Stock = 18, Price = 360000, ImageUrl = "/images/products/ao-co-tru-trang-m.jpg" },
                new ProductVariant { Id = 30, ProductId = 10, Size = "L", Color = "Trắng", Stock = 12, Price = 370000, ImageUrl = "/images/products/ao-co-tru-trang-l.jpg" },
                new ProductVariant { Id = 31, ProductId = 10, Size = "XL", Color = "Trắng", Stock = 8, Price = 375000, ImageUrl = "/images/products/ao-co-tru-trang-xl.jpg" },

                // Product 11
                new ProductVariant { Id = 32, ProductId = 11, Size = "30", Color = "Đen", Stock = 14, Price = 430000, ImageUrl = "/images/products/quan-slimfit-den-30.jpg" },
                new ProductVariant { Id = 33, ProductId = 11, Size = "32", Color = "Xám", Stock = 20, Price = 440000, ImageUrl = "/images/products/quan-slimfit-xam-32.jpg" },
                new ProductVariant { Id = 34, ProductId = 11, Size = "34", Color = "Đen", Stock = 15, Price = 445000, ImageUrl = "/images/products/quan-slimfit-den-34.jpg" },

                // Product 12
                new ProductVariant { Id = 35, ProductId = 12, Size = "S", Color = "Đen", Stock = 10, Price = 550000, ImageUrl = "/images/products/dam-body-den-s.jpg" },
                new ProductVariant { Id = 36, ProductId = 12, Size = "M", Color = "Đỏ", Stock = 8, Price = 560000, ImageUrl = "/images/products/dam-body-do-m.jpg" },
                new ProductVariant { Id = 37, ProductId = 12, Size = "L", Color = "Đen", Stock = 5, Price = 570000, ImageUrl = "/images/products/dam-body-den-l.jpg" },

                // Product 13
                new ProductVariant { Id = 38, ProductId = 13, Size = "M", Color = "Trắng", Stock = 20, Price = 480000, ImageUrl = "/images/products/ao-lua-nu-trang-m.jpg" },
                new ProductVariant { Id = 39, ProductId = 13, Size = "L", Color = "Trắng", Stock = 15, Price = 490000, ImageUrl = "/images/products/ao-lua-nu-trang-l.jpg" },
                new ProductVariant { Id = 40, ProductId = 13, Size = "XL", Color = "Hồng", Stock = 10, Price = 495000, ImageUrl = "/images/products/ao-lua-nu-hong-xl.jpg" },

                // Product 14
                new ProductVariant { Id = 41, ProductId = 14, Size = "32", Color = "Xám", Stock = 18, Price = 430000, ImageUrl = "/images/products/quan-basic-xam-32.jpg" },
                new ProductVariant { Id = 42, ProductId = 14, Size = "34", Color = "Đen", Stock = 20, Price = 435000, ImageUrl = "/images/products/quan-basic-den-34.jpg" },
                new ProductVariant { Id = 43, ProductId = 14, Size = "36", Color = "Xám", Stock = 15, Price = 440000, ImageUrl = "/images/products/quan-basic-xam-36.jpg" },

                // Product 15
                new ProductVariant { Id = 44, ProductId = 15, Size = "S", Color = "Hồng", Stock = 14, Price = 460000, ImageUrl = "/images/products/dam-hoa-nhi-hong-s.jpg" },
                new ProductVariant { Id = 45, ProductId = 15, Size = "M", Color = "Trắng", Stock = 10, Price = 470000, ImageUrl = "/images/products/dam-hoa-nhi-trang-m.jpg" },
                new ProductVariant { Id = 46, ProductId = 15, Size = "L", Color = "Xanh", Stock = 8, Price = 475000, ImageUrl = "/images/products/dam-hoa-nhi-xanh-l.jpg" },

                // Product 16
                new ProductVariant { Id = 47, ProductId = 16, Size = "M", Color = "Trắng", Stock = 20, Price = 370000, ImageUrl = "/images/products/ao-linen-trang-m.jpg" },
                new ProductVariant { Id = 48, ProductId = 16, Size = "L", Color = "Be", Stock = 15, Price = 375000, ImageUrl = "/images/products/ao-linen-be-l.jpg" },
                new ProductVariant { Id = 49, ProductId = 16, Size = "XL", Color = "Xám", Stock = 10, Price = 380000, ImageUrl = "/images/products/ao-linen-xam-xl.jpg" },

                // Product 17
                new ProductVariant { Id = 50, ProductId = 17, Size = "30", Color = "Nâu", Stock = 18, Price = 450000, ImageUrl = "/images/products/quan-kaki-nau-30.jpg" },
                new ProductVariant { Id = 51, ProductId = 17, Size = "32", Color = "Nâu", Stock = 20, Price = 455000, ImageUrl = "/images/products/quan-kaki-nau-32.jpg" },
                new ProductVariant { Id = 52, ProductId = 17, Size = "34", Color = "Xám", Stock = 15, Price = 460000, ImageUrl = "/images/products/quan-kaki-xam-34.jpg" },

                // Product 18
                new ProductVariant { Id = 53, ProductId = 18, Size = "S", Color = "Đen", Stock = 15, Price = 530000, ImageUrl = "/images/products/dam-voan-den-s.jpg" },
                new ProductVariant { Id = 54, ProductId = 18, Size = "M", Color = "Đỏ", Stock = 12, Price = 540000, ImageUrl = "/images/products/dam-voan-do-m.jpg" },
                new ProductVariant { Id = 55, ProductId = 18, Size = "L", Color = "Trắng", Stock = 10, Price = 550000, ImageUrl = "/images/products/dam-voan-trang-l.jpg" },

                // Product 19
                new ProductVariant { Id = 56, ProductId = 19, Size = "M", Color = "Xanh", Stock = 20, Price = 360000, ImageUrl = "/images/products/ao-thun-xanh-m.jpg" },
                new ProductVariant { Id = 57, ProductId = 19, Size = "L", Color = "Xanh", Stock = 15, Price = 365000, ImageUrl = "/images/products/ao-thun-xanh-l.jpg" },
                new ProductVariant { Id = 58, ProductId = 19, Size = "XL", Color = "Đen", Stock = 12, Price = 370000, ImageUrl = "/images/products/ao-thun-den-xl.jpg" },

                // Product 20
                new ProductVariant { Id = 59, ProductId = 20, Size = "32", Color = "Đen", Stock = 25, Price = 430000, ImageUrl = "/images/products/quan-jean-den-32.jpg" },
                new ProductVariant { Id = 60, ProductId = 20, Size = "34", Color = "Xám", Stock = 20, Price = 435000, ImageUrl = "/images/products/quan-jean-xam-34.jpg" },
                new ProductVariant { Id = 61, ProductId = 20, Size = "36", Color = "Xanh", Stock = 15, Price = 440000, ImageUrl = "/images/products/quan-jean-xanh-36.jpg" }
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
                new Cart { Id = 1, UserId = 1, CreatedAt = DateTime.Parse("2025-05-21") },
                new Cart { Id = 2, UserId = 1, CreatedAt = DateTime.Parse("2025-05-21") },
                new Cart { Id = 3, UserId = 2, CreatedAt = DateTime.Parse("2025-05-21") },
                new Cart { Id = 4, UserId = 2, CreatedAt = DateTime.Parse("2025-05-21") }
            );
            modelBuilder.Entity<CartItem>().HasData(
                new CartItem { Id = 1, CartId = 1, ProductVariantId = 1, Quantity = 3 },
                new CartItem { Id = 2, CartId = 1, ProductVariantId = 2, Quantity = 2 },
                new CartItem { Id = 3, CartId = 1, ProductVariantId = 3, Quantity = 5 },
                new CartItem { Id = 4, CartId = 2, ProductVariantId = 4, Quantity = 1 },
                new CartItem { Id = 5, CartId = 2, ProductVariantId = 5, Quantity = 3 },
                new CartItem { Id = 6, CartId = 2, ProductVariantId = 6, Quantity = 1 },
                new CartItem { Id = 7, CartId = 3, ProductVariantId = 7, Quantity = 2 }
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
                new Discount { Id = 1, Amount = 10, StartDate = DateTime.Parse("2025-06-01"), EndDate = DateTime.Parse("2025-06-30"), Status = true, ProductVariantId = 1 },
                new Discount { Id = 2, Amount = 15, StartDate = DateTime.Parse("2025-06-01"), EndDate = DateTime.Parse("2025-06-15"), Status = true, ProductVariantId = 2 },
                new Discount { Id = 3, Amount = 5, StartDate = DateTime.Parse("2025-06-10"), EndDate = DateTime.Parse("2025-07-10"), Status = false, ProductVariantId = 3 }
            );


        }
    }
}
