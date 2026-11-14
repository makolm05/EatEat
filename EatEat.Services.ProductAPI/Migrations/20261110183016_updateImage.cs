using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatEat.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagLocalPath",
                table: "Products",
                newName: "ImageLocalPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageLocalPath",
                table: "Products",
                newName: "ImagLocalPath");
        }
    }
}
