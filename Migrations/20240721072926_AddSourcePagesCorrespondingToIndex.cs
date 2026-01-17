using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class AddSourcePagesCorrespondingToIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "starting_index_page",
                table: "sources",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"UPDATE sources SET starting_index_page = starting_pdf_page");

            migrationBuilder.AddColumn<int>(
                name: "last_index_page",
                table: "sources",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"UPDATE sources SET last_index_page = last_pdf_page");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_index_page",
                table: "sources");

            migrationBuilder.DropColumn(
                name: "starting_index_page",
                table: "sources");
        }
    }
}
