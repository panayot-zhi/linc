using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class ElaborateDescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "notes",
                table: "sources",
                newName: "title_notes");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "sources",
                newName: "author_notes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "title_notes",
                table: "sources",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "author_notes",
                table: "sources",
                newName: "description");
        }
    }
}
