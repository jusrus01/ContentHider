using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContentHider.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAnonymizedFieldToRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnonymizedField",
                table: "Rules",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnonymizedField",
                table: "Rules");
        }
    }
}
