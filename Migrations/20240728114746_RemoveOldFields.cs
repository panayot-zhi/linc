using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class RemoveOldFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "asp_net_users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "asp_net_users",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "asp_net_users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "asp_net_users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "00000000-0000-0000-0000-000000000000",
                columns: new[] { "description", "first_name", "last_name" },
                values: new object[] { "System administrator. / Администратор на системата.", "Panayot", "Ivanov" });
        }
    }
}
