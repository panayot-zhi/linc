using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class SourceDossierRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "dossier_id",
                table: "sources",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_sources_dossier_id",
                table: "sources",
                column: "dossier_id",
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
            migrationBuilder.DropForeignKey(
                name: "fk_sources_dossiers_dossier_id",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_dossier_id",
                table: "sources");

            migrationBuilder.DropColumn(
                name: "dossier_id",
                table: "sources");
        }
    }
}
