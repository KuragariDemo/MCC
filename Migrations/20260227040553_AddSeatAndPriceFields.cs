using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatAndPriceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Feedbacks",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Feedbacks",
                newName: "SubmittedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Feedbacks",
                newName: "MemberId");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                table: "Feedbacks",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
