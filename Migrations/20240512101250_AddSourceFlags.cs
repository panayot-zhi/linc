using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class AddSourceFlags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "issue_theme",
                table: "sources",
                newName: "is_theme");

            migrationBuilder.AddColumn<bool>(
                name: "is_section",
                table: "sources",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_section",
                table: "sources");

            migrationBuilder.RenameColumn(
                name: "is_theme",
                table: "sources",
                newName: "issue_theme");
        }
    }
}
