using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCBShop.Migrations
{
    /// <inheritdoc />
    public partial class updateprice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ProductPrice",
                table: "Products",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ProductPrice",
                table: "Products",
                type: "decimal(8,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");
        }
    }
}
