using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class PopulateApplicationAuthors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migrate data from sources to authors (including dossier_id), excluding records with NULL first_name or last_name
            migrationBuilder.Sql(@"
                INSERT INTO authors (first_name, last_name, email, language_id, user_id, source_id, dossier_id, date_created, last_updated)
                SELECT 
                    first_name, 
                    last_name, 
                    NULL AS email, 
                    language_id, 
                    author_id AS user_id, 
                    id AS source_id, 
                    dossier_id, 
                    date_created, 
                    last_updated
                FROM sources
                WHERE first_name IS NOT NULL AND last_name IS NOT NULL
            ");

            // Migrate data from dossiers to authors, excluding already inserted records and records with NULL first_name or last_name
            migrationBuilder.Sql(@"
                INSERT INTO authors (first_name, last_name, email, language_id, user_id, dossier_id, date_created, last_updated)
                SELECT 
                    first_name, 
                    last_name, 
                    email, 
                    language_id, 
                    author_id AS user_id, 
                    id AS dossier_id, 
                    date_created, 
                    last_updated
                FROM dossiers
                WHERE id NOT IN (SELECT DISTINCT dossier_id FROM authors WHERE dossier_id IS NOT NULL) AND first_name IS NOT NULL AND last_name IS NOT NULL
            ");

            // Update all records in authors table: last_name to UPPER CASE, first_name to capitalize first letter only
            migrationBuilder.Sql(@"
                UPDATE authors
                SET 
                    last_name = UPPER(last_name),
                    first_name = CONCAT(UPPER(SUBSTRING(first_name, 1, 1)), LOWER(SUBSTRING(first_name, 2)))
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the migration by deleting the inserted data
            migrationBuilder.Sql("DELETE FROM authors WHERE dossier_id IS NOT NULL OR source_id IS NOT NULL");
        }
    }
}
