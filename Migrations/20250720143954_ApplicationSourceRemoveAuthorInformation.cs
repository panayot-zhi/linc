using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class ApplicationSourceRemoveAuthorInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sources_users_author_id",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_author_id",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_author_names",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_first_name",
                table: "sources");

            migrationBuilder.DropIndex(
                name: "ix_sources_last_name",
                table: "sources");

            migrationBuilder.DropColumn(
                name: "author_names",
                table: "sources");

            migrationBuilder.DropColumn(
                name: "author_id",
                table: "sources");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "sources");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "sources");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "author_id",
                table: "sources",
                type: "varchar(127)",
                maxLength: 127,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "sources",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "sources",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "author_names",
                table: "sources",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                computedColumnSql: "CONCAT(first_name, ' ', last_name)")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_sources_author_id",
                table: "sources",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_sources_author_names",
                table: "sources",
                column: "author_names");

            migrationBuilder.CreateIndex(
                name: "ix_sources_first_name",
                table: "sources",
                column: "first_name");

            migrationBuilder.CreateIndex(
                name: "ix_sources_last_name",
                table: "sources",
                column: "last_name");

            migrationBuilder.AddForeignKey(
                name: "fk_sources_users_author_id",
                table: "sources",
                column: "author_id",
                principalTable: "asp_net_users",
                principalColumn: "id");
        }
    }
}
