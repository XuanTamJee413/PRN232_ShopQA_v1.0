using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ProductId_Size_Color",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_Discount_Code",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "UsageLimit",
                table: "Discount");

            migrationBuilder.RenameColumn(
                name: "IsPercentage",
                table: "Discount",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "ProductVariants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "ProductVariants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductVariants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductVariantId",
                table: "Discount",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "City", "Country", "Street", "UserId" },
                values: new object[,]
                {
                    { 2, "Hải Phòng", "Việt Nam", "25 Điện Biên Phủ", 1 },
                    { 3, "Đà Nẵng", "Việt Nam", "56 Nguyễn Văn Linh", 2 }
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

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Áo thun");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Áo khoác");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Chân váy");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 4, "Quần jean" },
                    { 5, "Đồ thể thao" },
                    { 6, "Đồ ngủ" },
                    { 7, "Đồ công sở" },
                    { 8, "Giày dép" },
                    { 9, "Túi xách" },
                    { 10, "Phụ kiện thời trang" }
                });

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "ProductVariantId", "StartDate" },
                values: new object[] { new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Discount",
                columns: new[] { "Id", "Amount", "EndDate", "ProductVariantId", "StartDate", "Status" },
                values: new object[,]
                {
                    { 2, 15m, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { 3, 5m, new DateTime(2025, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false }
                });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "Price" },
                values: new object[] { "/images/products/ao-so-mi-trang-m.jpg", 350000m });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Color", "ImageUrl", "Price", "Stock" },
                values: new object[] { "Trắng", "/images/products/ao-so-mi-trang-l.jpg", 355000m, 15 });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Color", "ImageUrl", "Price", "ProductId", "Size", "Stock" },
                values: new object[] { "Xanh", "/images/products/ao-so-mi-trang-xl.jpg", 360000m, 1, "XL", 10 });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Color", "ImageUrl", "Price", "ProductId", "Size", "Stock" },
                values: new object[,]
                {
                    { 4, "Đỏ", "/images/products/ao-so-mi-trang-m-do.jpg", 355000m, 1, "M", 12 },
                    { 5, "Đen", "/images/products/quan-tay-den-32.jpg", 420000m, 2, "32", 10 },
                    { 6, "Xám", "/images/products/quan-tay-xam-34.jpg", 430000m, 2, "34", 15 },
                    { 7, "Đen", "/images/products/quan-tay-den-36.jpg", 435000m, 2, "36", 8 }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BrandId", "ImageUrl" },
                values: new object[] { 1, "/images/products/ao-so-mi-trang.jpg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BrandId", "ImageUrl" },
                values: new object[] { 2, "/images/products/quan-tay.jpg" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash", "Username" },
                values: new object[] { "tranb@gmail.com", "123", "tranthib" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "PasswordHash", "Role", "Username" },
                values: new object[] { "minhc@yahoo.com", "123", "Customer", "leminhc" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 3, "staff@shopqa.vn", "123", "Support", "staff" },
                    { 4, "thanhp@gmail.com", "123", "Customer", "phamthanh" },
                    { 5, "hoa.nguyen@gmail.com", "123", "Customer", "nguyenhoa" },
                    { 6, "admin@shopqa.vn", "123", "Moderator", "admin" }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "City", "Country", "Street", "UserId" },
                values: new object[,]
                {
                    { 4, "Hồ Chí Minh", "Việt Nam", "78 Trần Hưng Đạo", 4 },
                    { 5, "Hà Nội", "Việt Nam", "135 Nguyễn Văn Cừ", 4 },
                    { 6, "Cần Thơ", "Việt Nam", "89 Phan Đình Phùng", 5 },
                    { 7, "Bình Dương", "Việt Nam", "199 Cách Mạng Tháng Tám", 5 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
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
                table: "ProductVariants",
                columns: new[] { "Id", "Color", "ImageUrl", "Price", "ProductId", "Size", "Stock" },
                values: new object[,]
                {
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

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_ProductVariantId",
                table: "Discount",
                column: "ProductVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discount_ProductVariants_ProductVariantId",
                table: "Discount",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discount_ProductVariants_ProductVariantId",
                table: "Discount");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_Products_BrandId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Discount_ProductVariantId",
                table: "Discount");

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "Discount");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Discount",
                newName: "IsPercentage");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "ProductVariants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "ProductVariants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Discount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UsageLimit",
                table: "Discount",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Áo sơ mi");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Quần tây");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Đầm nữ");

            migrationBuilder.UpdateData(
                table: "Discount",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Code", "EndDate", "StartDate", "UsageLimit" },
                values: new object[] { "SALE10", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 100 });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Color", "ImageUrl", "Stock" },
                values: new object[] { "Xanh nhạt", null, 10 });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Color", "ImageUrl", "ProductId", "Size", "Stock" },
                values: new object[] { "Đen", null, 2, "32", 15 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "Price" },
                values: new object[] { null, 350000m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageUrl", "Price" },
                values: new object[] { null, 420000m });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash", "Username" },
                values: new object[] { "vana@gmail.com", "hash1", "nguyenvana" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "PasswordHash", "Role", "Username" },
                values: new object[] { "admin@shopqa.vn", "adminhash", "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId_Size_Color",
                table: "ProductVariants",
                columns: new[] { "ProductId", "Size", "Color" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discount_Code",
                table: "Discount",
                column: "Code",
                unique: true);
        }
    }
}
