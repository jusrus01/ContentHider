using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentHider.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFormatDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormatDefinition",
                table: "Formats",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormatDefinition",
                table: "Formats");
        }
    }
}
