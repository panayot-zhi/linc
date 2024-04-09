using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class AddSourcePdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "pdf_id",
                table: "sources",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_sources_pdf_id",
                table: "sources",
                column: "pdf_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_sources_documents_pdf_id",
                table: "sources",
                column: "pdf_id",
                principalTable: "documents",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sources_documents_pdf_id",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_pdf_id",
                table: "sources");

            migrationBuilder.DropColumn(
                name: "pdf_id",
                table: "sources");
        }
    }
}
