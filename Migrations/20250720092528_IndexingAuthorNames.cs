using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class IndexingAuthorNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_authors_names",
                table: "authors",
                column: "names");

            migrationBuilder.CreateIndex(
                name: "ix_authors_names_dossier_id",
                table: "authors",
                columns: new[] { "names", "dossier_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_authors_names_source_id",
                table: "authors",
                columns: new[] { "names", "source_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_authors_names_source_id_dossier_id",
                table: "authors",
                columns: new[] { "names", "source_id", "dossier_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_authors_names",
                table: "authors");

            migrationBuilder.DropIndex(
                name: "ix_authors_names_dossier_id",
                table: "authors");

            migrationBuilder.DropIndex(
                name: "ix_authors_names_source_id",
                table: "authors");

            migrationBuilder.DropIndex(
                name: "ix_authors_names_source_id_dossier_id",
                table: "authors");
        }
    }
}
