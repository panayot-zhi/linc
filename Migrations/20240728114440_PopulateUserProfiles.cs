using linc.Utility;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class PopulateUserProfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
INSERT INTO asp_net_user_profiles (language_id, user_id, first_name, last_name, description)
SELECT l.id, u.id, u.first_name, u.last_name, u.description 
FROM asp_net_users u
CROSS JOIN languages l
where u.id <> '{SiteConstant.ZeroGuid}';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // no need for down action since the whole table is dropped on the next migration.
        }
    }
}
