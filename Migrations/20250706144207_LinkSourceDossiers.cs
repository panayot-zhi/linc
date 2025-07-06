using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class LinkSourceDossiers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE sources
                JOIN dossiers d
                ON LOWER(d.title) = LOWER(sources.title)
                   AND LOWER(d.first_name) = LOWER(sources.first_name)
                   AND LOWER(d.last_name) = LOWER(sources.last_name)
                SET sources.dossier_id = d.id
                WHERE sources.id > 0;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE sources
                SET dossier_id = NULL;
            ");
        }
    }
}