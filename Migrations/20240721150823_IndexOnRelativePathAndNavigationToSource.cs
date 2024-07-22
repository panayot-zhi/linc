using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class IndexOnRelativePathAndNavigationToSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM documents WHERE document_type = 'SourcePdf';");
            migrationBuilder.Sql(@"DELETE FROM sources WHERE id > 0;");
            migrationBuilder.CreateIndex(
                name: "ix_documents_relative_path",
                table: "documents",
                column: "relative_path",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_documents_relative_path",
                table: "documents");
        }
    }
}
