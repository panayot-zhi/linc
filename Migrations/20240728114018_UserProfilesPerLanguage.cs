using linc.Utility;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class UserProfilesPerLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "preferred_language_id",
                table: "asp_net_users",
                type: "int",
                nullable: false,
                defaultValue: SiteConstant.BulgarianCulture.Key);

            migrationBuilder.CreateTable(
                name: "asp_net_user_profiles",
                columns: table => new
                {
                    language_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    first_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_profiles", x => new { x.user_id, x.language_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_profiles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_profiles_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "asp_net_user_profiles",
                columns: new[] { "language_id", "user_id", "description", "first_name", "last_name" },
                values: new object[] { 1, "00000000-0000-0000-0000-000000000000", null, null, null });

            migrationBuilder.InsertData(
                table: "asp_net_user_profiles",
                columns: new[] { "language_id", "user_id", "description", "first_name", "last_name" },
                values: new object[] { 2, "00000000-0000-0000-0000-000000000000", null, null, null });

            migrationBuilder.UpdateData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "00000000-0000-0000-0000-000000000000",
                column: "preferred_language_id",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_preferred_language_id",
                table: "asp_net_users",
                column: "preferred_language_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_profiles_language_id",
                table: "asp_net_user_profiles",
                column: "language_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_languages_preferred_language_id",
                table: "asp_net_users",
                column: "preferred_language_id",
                principalTable: "languages",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_languages_preferred_language_id",
                table: "asp_net_users");

            migrationBuilder.DropTable(
                name: "asp_net_user_profiles");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_preferred_language_id",
                table: "asp_net_users");

            migrationBuilder.DropColumn(
                name: "preferred_language_id",
                table: "asp_net_users");
        }
    }
}
