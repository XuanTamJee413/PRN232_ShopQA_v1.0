using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Users_UserId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Cart_CartId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_ProductVariants_ProductVariantId",
                table: "CartItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cart",
                table: "Cart");

            migrationBuilder.RenameTable(
                name: "CartItem",
                newName: "CartItems");

            migrationBuilder.RenameTable(
                name: "Cart",
                newName: "Carts");

            migrationBuilder.RenameIndex(
                name: "IX_CartItem_ProductVariantId",
                table: "CartItems",
                newName: "IX_CartItems_ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItem_CartId",
                table: "CartItems",
                newName: "IX_CartItems_CartId");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_UserId",
                table: "Carts",
                newName: "IX_Carts_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "Pending");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "Pending");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductVariants_ProductVariantId",
                table: "CartItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Carts_CartId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductVariants_ProductVariantId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItems",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Carts");

            migrationBuilder.RenameTable(
                name: "Carts",
                newName: "Cart");

            migrationBuilder.RenameTable(
                name: "CartItems",
                newName: "CartItem");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_UserId",
                table: "Cart",
                newName: "IX_Cart_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_ProductVariantId",
                table: "CartItem",
                newName: "IX_CartItem_ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItems_CartId",
                table: "CartItem",
                newName: "IX_CartItem_CartId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cart",
                table: "Cart",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Users_UserId",
                table: "Cart",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Cart_CartId",
                table: "CartItem",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_ProductVariants_ProductVariantId",
                table: "CartItem",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
