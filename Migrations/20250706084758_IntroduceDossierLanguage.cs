using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class IntroduceDossierLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "language_id",
                table: "dossiers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "ix_dossiers_language_id",
                table: "dossiers",
                column: "language_id");

            migrationBuilder.AddForeignKey(
                name: "fk_dossiers_languages_language_id",
                table: "dossiers",
                column: "language_id",
                principalTable: "languages",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dossiers_languages_language_id",
                table: "dossiers");

            migrationBuilder.DropIndex(
                name: "ix_dossiers_language_id",
                table: "dossiers");

            migrationBuilder.DropColumn(
                name: "language_id",
                table: "dossiers");
        }
    }
}
