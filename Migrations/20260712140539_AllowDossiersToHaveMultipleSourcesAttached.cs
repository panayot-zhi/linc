using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class AllowDossiersToHaveMultipleSourcesAttached : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sources_dossiers_dossier_id",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_dossier_id",
                table: "sources");

            migrationBuilder.CreateIndex(
                name: "ix_sources_dossier_id_language_id",
                table: "sources",
                columns: new[] { "dossier_id", "language_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_sources_dossiers_dossier_id",
                table: "sources",
                column: "dossier_id",
                principalTable: "dossiers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_sources_dossier_id_language_id",
                table: "sources");

            migrationBuilder.CreateIndex(
                name: "ix_sources_dossier_id",
                table: "sources",
                column: "dossier_id",
                unique: true);
        }
    }
}
