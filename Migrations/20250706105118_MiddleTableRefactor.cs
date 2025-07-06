using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class MiddleTableRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dossier_documents",
                columns: table => new
                {
                    dossier_id = table.Column<int>(type: "int", nullable: false),
                    document_id = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dossier_documents", x => new { x.document_id, x.dossier_id });
                    table.ForeignKey(
                        name: "fk_dossier_documents_document_id",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dossier_documents_dossier_id",
                        column: x => x.dossier_id,
                        principalTable: "dossiers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "issue_documents",
                columns: table => new
                {
                    issue_id = table.Column<int>(type: "int", nullable: false),
                    document_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_documents", x => new { x.document_id, x.issue_id });
                    table.ForeignKey(
                        name: "fk_issue_documents_document_id",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issue_documents_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_dossier_documents_dossier_id",
                table: "dossier_documents",
                column: "dossier_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_documents_issue_id",
                table: "issue_documents",
                column: "issue_id");

            // Migrate data from application_document_application_dossier to dossier_documents
            migrationBuilder.Sql(@"INSERT INTO dossier_documents (document_id, dossier_id)
                                  SELECT documents_id, dossiers_id FROM application_document_application_dossier");

            // Migrate data from application_document_application_issue to issue_documents
            migrationBuilder.Sql(@"INSERT INTO issue_documents (document_id, issue_id)
                                  SELECT files_id, issues_id FROM application_document_application_issue");

            migrationBuilder.DropTable(
                name: "application_document_application_dossier");

            migrationBuilder.DropTable(
                name: "application_document_application_issue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_document_application_dossier",
                columns: table => new
                {
                    documents_id = table.Column<int>(type: "int", nullable: false),
                    dossiers_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_document_application_dossier", x => new { x.documents_id, x.dossiers_id });
                    table.ForeignKey(
                        name: "fk_application_document_application_dossier_documents_documents",
                        column: x => x.documents_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_application_document_application_dossier_dossiers_dossiers_id",
                        column: x => x.dossiers_id,
                        principalTable: "dossiers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "application_document_application_issue",
                columns: table => new
                {
                    files_id = table.Column<int>(type: "int", nullable: false),
                    issues_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_document_application_issue", x => new { x.files_id, x.issues_id });
                    table.ForeignKey(
                        name: "fk_application_document_application_issue_documents_files_id",
                        column: x => x.files_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_application_document_application_issue_issues_issues_id",
                        column: x => x.issues_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Migrate data from dossier_documents to application_document_application_dossier
            migrationBuilder.Sql(@"INSERT INTO application_document_application_dossier (documents_id, dossiers_id)
                                  SELECT document_id, dossier_id FROM dossier_documents");

            // Migrate data from issue_documents to application_document_application_issue
            migrationBuilder.Sql(@"INSERT INTO application_document_application_issue (files_id, issues_id)
                                  SELECT document_id, issue_id FROM issue_documents");

            migrationBuilder.DropTable(
                name: "dossier_documents");

            migrationBuilder.DropTable(
                name: "issue_documents");
        }
    }
}
