using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class IndexAndUserTracing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "source_journals",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "issue_journals",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "created_by_id",
                table: "dossiers",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "edited_by_id",
                table: "dossiers",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "dossier_journals",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldMaxLength: 1024,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_source_journals_message",
                table: "source_journals",
                column: "message");

            migrationBuilder.CreateIndex(
                name: "ix_issue_journals_message",
                table: "issue_journals",
                column: "message");

            migrationBuilder.CreateIndex(
                name: "ix_dossiers_created_by_id",
                table: "dossiers",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_dossiers_edited_by_id",
                table: "dossiers",
                column: "edited_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_dossier_journals_message",
                table: "dossier_journals",
                column: "message");

            migrationBuilder.AddForeignKey(
                name: "fk_dossiers_users_created_by_id",
                table: "dossiers",
                column: "created_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_dossiers_users_edited_by_id",
                table: "dossiers",
                column: "edited_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dossiers_users_created_by_id",
                table: "dossiers");

            migrationBuilder.DropForeignKey(
                name: "fk_dossiers_users_edited_by_id",
                table: "dossiers");

            migrationBuilder.DropIndex(
                name: "ix_source_journals_message",
                table: "source_journals");

            migrationBuilder.DropIndex(
                name: "ix_issue_journals_message",
                table: "issue_journals");

            migrationBuilder.DropIndex(
                name: "ix_dossiers_created_by_id",
                table: "dossiers");

            migrationBuilder.DropIndex(
                name: "ix_dossiers_edited_by_id",
                table: "dossiers");

            migrationBuilder.DropIndex(
                name: "ix_dossier_journals_message",
                table: "dossier_journals");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "dossiers");

            migrationBuilder.DropColumn(
                name: "edited_by_id",
                table: "dossiers");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "source_journals",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "issue_journals",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "message",
                table: "dossier_journals",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
