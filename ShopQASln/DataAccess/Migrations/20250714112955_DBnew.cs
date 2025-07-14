using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DBnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishlistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishlistId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Wishlists_WishlistId",
                        column: x => x.WishlistId,
                        principalTable: "Wishlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discount_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Việt Tiến" },
                    { 2, "An Phước" },
                    { 3, "May 10" },
                    { 4, "Blue Exchange" },
                    { 5, "Routine" },
                    { 6, "Ninomaxx" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Áo thun" },
                    { 2, "Áo khoác" },
                    { 3, "Chân váy" },
                    { 4, "Quần jean" },
                    { 5, "Đồ thể thao" },
                    { 6, "Đồ ngủ" },
                    { 7, "Đồ công sở" },
                    { 8, "Giày dép" },
                    { 9, "Túi xách" },
                    { 10, "Phụ kiện thời trang" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "Role", "Status", "Username" },
                values: new object[,]
                {
                    { 1, "tranb@gmail.com", "123", "Customer", "Active", "tranthib" },
                    { 2, "minhc@yahoo.com", "123", "Customer", "Active", "leminhc" },
                    { 3, "staff@shopqa.vn", "123", "Staff", "Active", "staff" },
                    { 4, "thanhp@gmail.com", "123", "Customer", "Active", "phamthanh" },
                    { 5, "hoa.nguyen@gmail.com", "123", "Customer", "Active", "nguyenhoa" },
                    { 6, "admin@shopqa.vn", "123", "Admin", "Active", "admin" }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "City", "Country", "Street", "UserId" },
                values: new object[,]
                {
                    { 1, "Hà Nội", "Việt Nam", "123 Nguyễn Trãi", 1 },
                    { 2, "Hải Phòng", "Việt Nam", "25 Điện Biên Phủ", 1 },
                    { 3, "Đà Nẵng", "Việt Nam", "56 Nguyễn Văn Linh", 2 },
                    { 4, "Hồ Chí Minh", "Việt Nam", "78 Trần Hưng Đạo", 4 },
                    { 5, "Hà Nội", "Việt Nam", "135 Nguyễn Văn Cừ", 4 },
                    { 6, "Cần Thơ", "Việt Nam", "89 Phan Đình Phùng", 5 },
                    { 7, "Bình Dương", "Việt Nam", "199 Cách Mạng Tháng Tám", 5 }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "CreatedAt", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1 },
                    { 2, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1 },
                    { 3, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2 },
                    { 4, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "OrderDate", "TotalAmount", "UserId" },
                values: new object[] { 1, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 770000m, 1 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, 1, 1, "Form Hàn Quốc, vải lụa mát, thích hợp đi học và đi làm.", "/images/products/ao-so-mi-trang.jpg", "Áo sơ mi trắng nam" },
                    { 2, 2, 2, "Vải co giãn, mặc nhẹ thoáng mát.", "/images/products/quan-tay.jpg", "Quần tây công sở" },
                    { 3, 3, 3, "Đầm nữ dáng xòe, chất liệu cao cấp.", "/images/products/dam-nu.jpg", "Đầm nữ công sở" },
                    { 4, 4, 1, "Áo sơ mi caro trẻ trung, cá tính.", "/images/products/ao-so-mi-caro.jpg", "Áo sơ mi caro nam" },
                    { 5, 5, 2, "Phong cách thanh lịch, hợp thời trang công sở.", "/images/products/quan-tay-ong-dung.jpg", "Quần tây ống đứng" },
                    { 6, 6, 3, "Chất cotton thoáng mát, kiểu dáng đơn giản.", "/images/products/dam-suong.jpg", "Đầm suông tay lỡ" },
                    { 7, 5, 1, "Vải cotton, ít nhăn, dễ ủi, thoải mái vận động.", "/images/products/ao-so-mi-xanh.jpg", "Áo sơ mi xanh dương" },
                    { 8, 3, 2, "Thiết kế giúp tôn dáng, chất vải cao cấp.", "/images/products/quan-tay-nu.jpg", "Quần tây lưng cao nữ" },
                    { 9, 2, 3, "Dáng ôm body, phong cách sang trọng.", "/images/products/dam-ren.jpg", "Đầm dự tiệc ren hoa" },
                    { 10, 6, 1, "Phong cách trẻ trung, lịch sự, dễ phối đồ.", "/images/products/ao-co-tru.jpg", "Áo sơ mi cổ trụ nam" },
                    { 11, 1, 2, "Tôn dáng, chất liệu mềm mại, co giãn tốt.", "/images/products/quan-slimfit.jpg", "Quần tây slimfit nam" },
                    { 12, 4, 3, "Dáng ôm body, tôn dáng, phù hợp dự tiệc.", "/images/products/dam-body.jpg", "Đầm body tay dài ren" },
                    { 13, 5, 1, "Vải lụa cao cấp, mềm mại, không nhăn.", "/images/products/ao-lua-nu.jpg", "Áo sơ mi lụa nữ cao cấp" },
                    { 14, 2, 2, "Màu xám, đơn giản, thích hợp đi làm.", "/images/products/quan-basic.jpg", "Quần tây nam màu xám" },
                    { 15, 6, 3, "Phong cách dễ thương, nhẹ nhàng.", "/images/products/dam-hoa-nhi.jpg", "Đầm hoa nhí xòe nhẹ" },
                    { 16, 3, 1, "Thoáng mát, phù hợp mùa hè.", "/images/products/ao-linen.jpg", "Áo sơ mi linen nam trắng" },
                    { 17, 4, 2, "Màu nâu, chất kaki, co giãn nhẹ.", "/images/products/quan-kaki.jpg", "Quần kaki công sở" },
                    { 18, 5, 3, "Phong cách công chúa, phù hợp dạo phố.", "/images/products/dam-maxi.jpg", "Đầm maxi tay bồng trắng" },
                    { 19, 2, 1, "Form rộng, thoải mái, trẻ trung.", "/images/products/ao-caro-nu.jpg", "Áo sơ mi sọc caro nữ form rộng" },
                    { 20, 1, 2, "Thiết kế thời trang, mát mẻ cho mùa hè.", "/images/products/quan-lung.jpg", "Quần lửng nữ mùa hè" }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "Method", "OrderId", "PaidAt", "Status" },
                values: new object[] { 1, 770000m, "COD", 1, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Completed" });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Color", "ImageUrl", "Price", "ProductId", "Size", "Stock" },
                values: new object[,]
                {
                    { 1, "Trắng", "/images/products/ao-so-mi-trang-m.jpg", 350000m, 1, "M", 20 },
                    { 2, "Trắng", "/images/products/ao-so-mi-trang-l.jpg", 355000m, 1, "L", 15 },
                    { 3, "Xanh", "/images/products/ao-so-mi-trang-xl.jpg", 360000m, 1, "XL", 10 },
                    { 4, "Đỏ", "/images/products/ao-so-mi-trang-m-do.jpg", 355000m, 1, "M", 12 },
                    { 5, "Đen", "/images/products/quan-tay-den-32.jpg", 420000m, 2, "32", 10 },
                    { 6, "Xám", "/images/products/quan-tay-xam-34.jpg", 430000m, 2, "34", 15 },
                    { 7, "Đen", "/images/products/quan-tay-den-36.jpg", 435000m, 2, "36", 8 },
                    { 8, "Đỏ", "/images/products/dam-nu-do-s.jpg", 500000m, 3, "S", 8 },
                    { 9, "Đen", "/images/products/dam-nu-den-m.jpg", 520000m, 3, "M", 5 },
                    { 10, "Trắng", "/images/products/dam-nu-trang-l.jpg", 530000m, 3, "L", 3 },
                    { 11, "Caro Xanh", "/images/products/ao-so-mi-caro-xanh-m.jpg", 370000m, 4, "M", 20 },
                    { 12, "Caro Đỏ", "/images/products/ao-so-mi-caro-do-l.jpg", 375000m, 4, "L", 15 },
                    { 13, "Caro Đen", "/images/products/ao-so-mi-caro-den-xl.jpg", 380000m, 4, "XL", 10 },
                    { 14, "Xám", "/images/products/quan-tay-xam-30.jpg", 440000m, 5, "30", 18 },
                    { 15, "Đen", "/images/products/quan-tay-den-32.jpg", 445000m, 5, "32", 20 },
                    { 16, "Xám", "/images/products/quan-tay-xam-34.jpg", 450000m, 5, "34", 12 },
                    { 17, "Be", "/images/products/dam-suong-be-m.jpg", 460000m, 6, "M", 14 },
                    { 18, "Trắng", "/images/products/dam-suong-trang-l.jpg", 470000m, 6, "L", 16 },
                    { 19, "Xanh", "/images/products/dam-suong-xanh-xl.jpg", 480000m, 6, "XL", 10 },
                    { 20, "Xanh Dương", "/images/products/ao-so-mi-xanh-m.jpg", 355000m, 7, "M", 25 },
                    { 21, "Xanh Dương", "/images/products/ao-so-mi-xanh-l.jpg", 360000m, 7, "L", 20 },
                    { 22, "Xanh Dương", "/images/products/ao-so-mi-xanh-xl.jpg", 370000m, 7, "XL", 10 },
                    { 23, "Đen", "/images/products/quan-tay-nu-den-s.jpg", 480000m, 8, "S", 10 },
                    { 24, "Xám", "/images/products/quan-tay-nu-xam-m.jpg", 490000m, 8, "M", 15 },
                    { 25, "Đỏ", "/images/products/quan-tay-nu-do-l.jpg", 495000m, 8, "L", 12 },
                    { 26, "Đỏ", "/images/products/dam-ren-do-m.jpg", 520000m, 9, "M", 8 },
                    { 27, "Đỏ", "/images/products/dam-ren-do-l.jpg", 530000m, 9, "L", 5 },
                    { 28, "Đỏ", "/images/products/dam-ren-do-xl.jpg", 540000m, 9, "XL", 3 },
                    { 29, "Trắng", "/images/products/ao-co-tru-trang-m.jpg", 360000m, 10, "M", 18 },
                    { 30, "Trắng", "/images/products/ao-co-tru-trang-l.jpg", 370000m, 10, "L", 12 },
                    { 31, "Trắng", "/images/products/ao-co-tru-trang-xl.jpg", 375000m, 10, "XL", 8 },
                    { 32, "Đen", "/images/products/quan-slimfit-den-30.jpg", 430000m, 11, "30", 14 },
                    { 33, "Xám", "/images/products/quan-slimfit-xam-32.jpg", 440000m, 11, "32", 20 },
                    { 34, "Đen", "/images/products/quan-slimfit-den-34.jpg", 445000m, 11, "34", 15 },
                    { 35, "Đen", "/images/products/dam-body-den-s.jpg", 550000m, 12, "S", 10 },
                    { 36, "Đỏ", "/images/products/dam-body-do-m.jpg", 560000m, 12, "M", 8 },
                    { 37, "Đen", "/images/products/dam-body-den-l.jpg", 570000m, 12, "L", 5 },
                    { 38, "Trắng", "/images/products/ao-lua-nu-trang-m.jpg", 480000m, 13, "M", 20 },
                    { 39, "Trắng", "/images/products/ao-lua-nu-trang-l.jpg", 490000m, 13, "L", 15 },
                    { 40, "Hồng", "/images/products/ao-lua-nu-hong-xl.jpg", 495000m, 13, "XL", 10 },
                    { 41, "Xám", "/images/products/quan-basic-xam-32.jpg", 430000m, 14, "32", 18 },
                    { 42, "Đen", "/images/products/quan-basic-den-34.jpg", 435000m, 14, "34", 20 },
                    { 43, "Xám", "/images/products/quan-basic-xam-36.jpg", 440000m, 14, "36", 15 },
                    { 44, "Hồng", "/images/products/dam-hoa-nhi-hong-s.jpg", 460000m, 15, "S", 14 },
                    { 45, "Trắng", "/images/products/dam-hoa-nhi-trang-m.jpg", 470000m, 15, "M", 10 },
                    { 46, "Xanh", "/images/products/dam-hoa-nhi-xanh-l.jpg", 475000m, 15, "L", 8 },
                    { 47, "Trắng", "/images/products/ao-linen-trang-m.jpg", 370000m, 16, "M", 20 },
                    { 48, "Be", "/images/products/ao-linen-be-l.jpg", 375000m, 16, "L", 15 },
                    { 49, "Xám", "/images/products/ao-linen-xam-xl.jpg", 380000m, 16, "XL", 10 },
                    { 50, "Nâu", "/images/products/quan-kaki-nau-30.jpg", 450000m, 17, "30", 18 },
                    { 51, "Nâu", "/images/products/quan-kaki-nau-32.jpg", 455000m, 17, "32", 20 },
                    { 52, "Xám", "/images/products/quan-kaki-xam-34.jpg", 460000m, 17, "34", 15 },
                    { 53, "Đen", "/images/products/dam-voan-den-s.jpg", 530000m, 18, "S", 15 },
                    { 54, "Đỏ", "/images/products/dam-voan-do-m.jpg", 540000m, 18, "M", 12 },
                    { 55, "Trắng", "/images/products/dam-voan-trang-l.jpg", 550000m, 18, "L", 10 },
                    { 56, "Xanh", "/images/products/ao-thun-xanh-m.jpg", 360000m, 19, "M", 20 },
                    { 57, "Xanh", "/images/products/ao-thun-xanh-l.jpg", 365000m, 19, "L", 15 },
                    { 58, "Đen", "/images/products/ao-thun-den-xl.jpg", 370000m, 19, "XL", 12 },
                    { 59, "Đen", "/images/products/quan-jean-den-32.jpg", 430000m, 20, "32", 25 },
                    { 60, "Xám", "/images/products/quan-jean-xam-34.jpg", 435000m, 20, "34", 20 },
                    { 61, "Xanh", "/images/products/quan-jean-xanh-36.jpg", 440000m, 20, "36", 15 }
                });

            migrationBuilder.InsertData(
                table: "Review",
                columns: new[] { "Id", "Comment", "CreatedAt", "ProductId", "Rating", "UserId" },
                values: new object[] { 1, "Sản phẩm rất tốt!", new DateTime(2025, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, 1 });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "Id", "CartId", "ProductVariantId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 1, 3 },
                    { 2, 1, 2, 2 },
                    { 3, 1, 3, 5 },
                    { 4, 2, 4, 1 },
                    { 5, 2, 5, 3 },
                    { 6, 2, 6, 1 },
                    { 7, 3, 7, 2 }
                });

            migrationBuilder.InsertData(
                table: "Discount",
                columns: new[] { "Id", "Amount", "EndDate", "ProductVariantId", "StartDate", "Status" },
                values: new object[,]
                {
                    { 1, 10m, new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { 2, 15m, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { 3, 5m, new DateTime(2025, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false }
                });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "Id", "ProductVariantId", "Quantity", "UpdatedAt" },
                values: new object[] { 1, 1, 20, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "Price", "ProductVariantId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 350000m, 1, 1 },
                    { 2, 1, 420000m, 3, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductVariantId",
                table: "CartItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discount_ProductVariantId",
                table: "Discount",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductVariantId",
                table: "Inventory",
                column: "ProductVariantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductVariantId",
                table: "OrderItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ProductId",
                table: "Review",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                table: "Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_ProductId",
                table: "WishlistItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_WishlistId",
                table: "WishlistItems",
                column: "WishlistId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "WishlistItems");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
