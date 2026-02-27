using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HighPrice",
                table: "Events",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "HighSeats",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "LowPrice",
                table: "Events",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "LowSeats",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MediumPrice",
                table: "Events",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "MediumSeats",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighPrice",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "HighSeats",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LowPrice",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LowSeats",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MediumPrice",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MediumSeats",
                table: "Events");
        }
    }
}
