using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class RobustSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sources_issues_issue_id",
                table: "sources");

            migrationBuilder.AlterColumn<int>(
                name: "issue_id",
                table: "sources",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "author_names",
                table: "sources",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "fk_sources_issues_issue_id",
                table: "sources",
                column: "issue_id",
                principalTable: "issues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sources_issues_issue_id",
                table: "sources");

            migrationBuilder.DropColumn(
                name: "author_names",
                table: "sources");

            migrationBuilder.AlterColumn<int>(
                name: "issue_id",
                table: "sources",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "fk_sources_issues_issue_id",
                table: "sources",
                column: "issue_id",
                principalTable: "issues",
                principalColumn: "id");
        }
    }
}
