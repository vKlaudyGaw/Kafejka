using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kafejka.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodanieLoyaltiesController : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoyaltyId",
                table: "Transaction",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Loyalty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalPoints = table.Column<int>(type: "int", nullable: false),
                    NumberOfStampsUses = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loyalty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loyalty_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_LoyaltyId",
                table: "Transaction",
                column: "LoyaltyId");

            migrationBuilder.CreateIndex(
                name: "IX_Loyalty_UserId",
                table: "Loyalty",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Loyalty_LoyaltyId",
                table: "Transaction",
                column: "LoyaltyId",
                principalTable: "Loyalty",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Loyalty_LoyaltyId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "Loyalty");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_LoyaltyId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "LoyaltyId",
                table: "Transaction");
        }
    }
}
