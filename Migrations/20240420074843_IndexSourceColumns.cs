using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class IndexSourceColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_sources_author_names",
                table: "sources",
                column: "author_names");

            migrationBuilder.CreateIndex(
                name: "ix_sources_title",
                table: "sources",
                column: "title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sources_author_names",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_title",
                table: "sources");
        }
    }
}
