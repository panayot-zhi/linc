using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class RenameAuthorNotesToAuthorsNotesOnSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "author_notes",
                table: "sources",
                newName: "authors_notes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "authors_notes",
                table: "sources",
                newName: "author_notes");
        }
    }
}
