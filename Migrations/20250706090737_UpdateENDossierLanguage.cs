using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class UpdateENDossierLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE dossiers SET language_id = 2 WHERE title NOT REGEXP '[А-Яа-я]' AND id > 0;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // do nothing, column will be dropped in the next migration down
        }
    }
}
