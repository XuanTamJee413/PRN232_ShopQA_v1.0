using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventory_ProductVariantId",
                table: "Inventory");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Cart",
                columns: new[] { "Id", "CreatedAt", "UserId" },
                values: new object[,]
                {
                    { 2, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.UpdateData(
                table: "CartItem",
                keyColumn: "Id",
                keyValue: 1,
                column: "Quantity",
                value: 3);

            migrationBuilder.InsertData(
                table: "CartItem",
                columns: new[] { "Id", "CartId", "ProductVariantId", "Quantity" },
                values: new object[,]
                {
                    { 2, 1, 2, 2 },
                    { 3, 1, 3, 5 }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Role", "Status" },
                values: new object[] { "Staff", "Active" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Status",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Role", "Status" },
                values: new object[] { "Admin", "Active" });

            migrationBuilder.InsertData(
                table: "CartItem",
                columns: new[] { "Id", "CartId", "ProductVariantId", "Quantity" },
                values: new object[,]
                {
                    { 4, 2, 4, 1 },
                    { 5, 2, 5, 3 },
                    { 6, 2, 6, 1 },
                    { 7, 3, 7, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductVariantId",
                table: "Inventory",
                column: "ProductVariantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventory_ProductVariantId",
                table: "Inventory");

            migrationBuilder.DeleteData(
                table: "Cart",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CartItem",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CartItem",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CartItem",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CartItem",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CartItem",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CartItem",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cart",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cart",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "CartItem",
                keyColumn: "Id",
                keyValue: 1,
                column: "Quantity",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "Role",
                value: "Support");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "Role",
                value: "Moderator");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductVariantId",
                table: "Inventory",
                column: "ProductVariantId");
        }
    }
}
