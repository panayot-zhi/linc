using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class RemoveEditedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dossiers_users_edited_by_id",
                table: "dossiers");

            migrationBuilder.DropIndex(
                name: "ix_dossiers_edited_by_id",
                table: "dossiers");

            migrationBuilder.DropColumn(
                name: "edited_by_id",
                table: "dossiers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "edited_by_id",
                table: "dossiers",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_dossiers_edited_by_id",
                table: "dossiers",
                column: "edited_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_dossiers_users_edited_by_id",
                table: "dossiers",
                column: "edited_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id");
        }
    }
}
