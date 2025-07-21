using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class ApplicationAuthorPublicationAgreement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "agreement_id",
                table: "authors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_authors_agreement_id",
                table: "authors",
                column: "agreement_id");

            migrationBuilder.AddForeignKey(
                name: "fk_authors_documents_agreement_id",
                table: "authors",
                column: "agreement_id",
                principalTable: "documents",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_authors_documents_agreement_id",
                table: "authors");

            migrationBuilder.DropIndex(
                name: "ix_authors_agreement_id",
                table: "authors");

            migrationBuilder.DropColumn(
                name: "agreement_id",
                table: "authors");
        }
    }
}
