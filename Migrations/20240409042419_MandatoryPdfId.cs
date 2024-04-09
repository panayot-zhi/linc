using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class MandatoryPdfId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sources_documents_pdf_id",
                table: "sources");

            migrationBuilder.AlterColumn<int>(
                name: "pdf_id",
                table: "sources",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<int>(
                name: "pdf_id",
                table: "sources",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "fk_sources_documents_pdf_id",
                table: "sources",
                column: "pdf_id",
                principalTable: "documents",
                principalColumn: "id");
        }
    }
}
