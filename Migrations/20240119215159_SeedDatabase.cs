using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class SeedDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "asp_net_roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "00000000-0000-0000-0000-000000000000", "000000000000-0000-0000-0000-00000000", "ADMINISTRATOR", "ADMINISTRATOR" },
                    { "05cbe4c7-108e-40bc-bee7-65438875026e", "e62057883456-7eeb-cb04-e801-7c4ebc50", "EDITOR", "EDITOR" },
                    { "5e1199d7-7725-4900-aa34-5496365bf5a0", "0a5fb5636945-43aa-0094-5277-7d9911e5", "HEAD_EDITOR", "HEAD_EDITOR" },
                    { "6b1acea8-2d26-4c82-b6ad-7281b7d621ae", "ea126d7b1827-da6b-28c4-62d2-8aeca1b6", "USER_PLUS", "USER_PLUS" },
                    { "90667439-9058-4956-96e6-d23bac481443", "344184cab32d-6e69-6594-8509-93476609", "USER", "USER" }
                });

            migrationBuilder.InsertData(
                table: "asp_net_users",
                columns: new[] { "id", "access_failed_count", "avatar_type", "concurrency_stamp", "date_created", "description", "display_email", "display_name_type", "email", "email_confirmed", "facebook_avatar_path", "first_name", "google_avatar_path", "internal_avatar_path", "last_name", "last_updated", "lockout_enabled", "lockout_end", "normalized_email", "normalized_user_name", "password_hash", "phone_number", "phone_number_confirmed", "security_stamp", "twitter_avatar_path", "two_factor_enabled", "user_name" },
                values: new object[] { "00000000-0000-0000-0000-000000000000", 0, 1, "00000000-0000-0000-0000-000000000000", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System administrator. / Администратор на системата.", true, 2, "admin-linc@uni-plovdiv.bg", true, null, "Panayot", null, null, "Ivanov", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "ADMIN-LINC@UNI-PLOVDIV.BG", "P.IVANOV", "CHANGE_ME", null, false, "00000000-0000-0000-0000-000000000000", null, false, "p.ivanov" });

            migrationBuilder.InsertData(
                table: "languages",
                columns: new[] { "id", "culture" },
                values: new object[,]
                {
                    { 1, "bg" },
                    { 2, "en" }
                });

            migrationBuilder.InsertData(
                table: "asp_net_user_roles",
                columns: new[] { "role_id", "user_id" },
                values: new object[] { "00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "asp_net_roles",
                keyColumn: "id",
                keyValue: "05cbe4c7-108e-40bc-bee7-65438875026e");

            migrationBuilder.DeleteData(
                table: "asp_net_roles",
                keyColumn: "id",
                keyValue: "5e1199d7-7725-4900-aa34-5496365bf5a0");

            migrationBuilder.DeleteData(
                table: "asp_net_roles",
                keyColumn: "id",
                keyValue: "6b1acea8-2d26-4c82-b6ad-7281b7d621ae");

            migrationBuilder.DeleteData(
                table: "asp_net_roles",
                keyColumn: "id",
                keyValue: "90667439-9058-4956-96e6-d23bac481443");

            migrationBuilder.DeleteData(
                table: "asp_net_user_roles",
                keyColumns: new[] { "role_id", "user_id" },
                keyValues: new object[] { "00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000" });

            migrationBuilder.DeleteData(
                table: "languages",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "languages",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "asp_net_roles",
                keyColumn: "id",
                keyValue: "00000000-0000-0000-0000-000000000000");

            migrationBuilder.DeleteData(
                table: "asp_net_users",
                keyColumn: "id",
                keyValue: "00000000-0000-0000-0000-000000000000");
        }
    }
}
