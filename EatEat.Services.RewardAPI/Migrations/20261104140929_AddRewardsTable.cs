using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatEat.Services.RewardAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    RewardsActivity = table.Column<int>(type: "int", nullable: false),
                    RewardsDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rewards");
        }
    }
}
