using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class DocumentsForMoreThanOneSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sources_documents_pdf_id",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_pdf_id",
                table: "sources");

            migrationBuilder.CreateIndex(
                name: "ix_sources_pdf_id",
                table: "sources",
                column: "pdf_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sources_documents_pdf_id",
                table: "sources",
                column: "pdf_id",
                principalTable: "documents",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sources_documents_pdf_id",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_pdf_id",
                table: "sources");

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
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
