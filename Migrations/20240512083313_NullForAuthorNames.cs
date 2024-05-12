using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class NullForAuthorNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "author_names",
                table: "sources",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                computedColumnSql: "CONCAT(first_name, ' ', last_name)",
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldComputedColumnSql: "CONCAT(first_name, ' ', last_name)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "sources",
                keyColumn: "author_names",
                keyValue: null,
                column: "author_names",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "author_names",
                table: "sources",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                computedColumnSql: "CONCAT(first_name, ' ', last_name)",
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComputedColumnSql: "CONCAT(first_name, ' ', last_name)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
