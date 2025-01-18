using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kafejka.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProblemAppDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Transaction",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Transaction");
        }
    }
}
