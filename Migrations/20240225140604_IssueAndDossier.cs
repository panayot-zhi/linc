using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class IssueAndDossier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    extension = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mime_type = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    document_type = table.Column<int>(type: "int", nullable: false),
                    relative_path = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_documents", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "dossiers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    first_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dossiers", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "issues",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    issue_number = table.Column<int>(type: "int", nullable: false),
                    release_year = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issues", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "sources",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    first_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    title = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notes = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    starting_page = table.Column<int>(type: "int", nullable: false),
                    language_id = table.Column<int>(type: "int", nullable: false),
                    issue_id = table.Column<int>(type: "int", nullable: true),
                    author_id = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sources", x => x.id);
                    table.ForeignKey(
                        name: "fk_sources_issues_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_sources_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sources_users_author_id",
                        column: x => x.author_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_application_document_application_dossier_dossiers_id",
                table: "application_document_application_dossier",
                column: "dossiers_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_document_application_issue_issues_id",
                table: "application_document_application_issue",
                column: "issues_id");

            migrationBuilder.CreateIndex(
                name: "ix_sources_author_id",
                table: "sources",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_sources_issue_id",
                table: "sources",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_sources_language_id",
                table: "sources",
                column: "language_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_document_application_dossier");

            migrationBuilder.DropTable(
                name: "application_document_application_issue");

            migrationBuilder.DropTable(
                name: "sources");

            migrationBuilder.DropTable(
                name: "dossiers");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "issues");
        }
    }
}
