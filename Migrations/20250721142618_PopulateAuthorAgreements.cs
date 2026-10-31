using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class PopulateAuthorAgreements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE authors a
                JOIN dossiers d ON a.dossier_id = d.id
                JOIN dossier_documents dd ON d.id = dd.dossier_id
                JOIN documents doc ON dd.document_id = doc.id
                SET a.agreement_id = doc.id
                WHERE doc.document_type = 'Agreement'
            ");

            migrationBuilder.Sql(@"
                DELETE dd
                FROM dossier_documents dd
                INNER JOIN authors a ON a.dossier_id = dd.dossier_id
                WHERE a.agreement_id = dd.document_id
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE authors a
                SET a.agreement_id = NULL                
            ");
        }
    }
}
