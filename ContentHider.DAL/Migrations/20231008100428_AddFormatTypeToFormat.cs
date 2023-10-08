using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentHider.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFormatTypeToFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Formats",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Formats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Formats");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Formats");
        }
    }
}
