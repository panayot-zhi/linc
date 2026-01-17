using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class TieSourcePagesWithPdfPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "starting_page",
                table: "sources",
                newName: "starting_pdf_page");

            migrationBuilder.RenameColumn(
                name: "last_page",
                table: "sources",
                newName: "last_pdf_page");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "starting_pdf_page",
                table: "sources",
                newName: "starting_page");

            migrationBuilder.RenameColumn(
                name: "last_pdf_page",
                table: "sources",
                newName: "last_page");
        }
    }
}
