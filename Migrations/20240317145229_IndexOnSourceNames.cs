using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class IndexOnSourceNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_sources_first_name",
                table: "sources",
                column: "first_name");

            migrationBuilder.CreateIndex(
                name: "ix_sources_last_name",
                table: "sources",
                column: "last_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sources_first_name",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_last_name",
                table: "sources");
        }
    }
}
