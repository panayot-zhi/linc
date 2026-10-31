using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class AuthorNamesComputedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "names",
                table: "authors",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                computedColumnSql: "CONCAT(first_name, ' ', last_name)")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "names",
                table: "authors");
        }
    }
}
