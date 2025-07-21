using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class CutOffDossierAuthorInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dossiers_users_author_id",
                table: "dossiers");

            migrationBuilder.DropIndex(
                name: "ix_dossiers_author_id",
                table: "dossiers");

            migrationBuilder.DropColumn(
                name: "author_id",
                table: "dossiers");

            migrationBuilder.DropColumn(
                name: "email",
                table: "dossiers");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "dossiers");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "dossiers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "author_id",
                table: "dossiers",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "dossiers",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "dossiers",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "dossiers",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_dossiers_author_id",
                table: "dossiers",
                column: "author_id");

            migrationBuilder.AddForeignKey(
                name: "fk_dossiers_users_author_id",
                table: "dossiers",
                column: "author_id",
                principalTable: "asp_net_users",
                principalColumn: "id");
        }
    }
}
