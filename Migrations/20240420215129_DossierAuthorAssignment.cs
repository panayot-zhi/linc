using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class DossierAuthorAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "author_id",
                table: "dossiers",
                type: "varchar(255)",
                nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
