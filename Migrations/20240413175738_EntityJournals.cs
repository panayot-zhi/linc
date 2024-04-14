using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class EntityJournals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dossier_journals",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    dossier_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    performed_by_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dossier_journals", x => x.id);
                    table.ForeignKey(
                        name: "fk_dossier_journals_dossiers_dossier_id",
                        column: x => x.dossier_id,
                        principalTable: "dossiers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dossier_journals_users_performed_by_id",
                        column: x => x.performed_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "issue_journals",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    issue_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    performed_by_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_journals", x => x.id);
                    table.ForeignKey(
                        name: "fk_issue_journals_issues_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issue_journals_users_performed_by_id",
                        column: x => x.performed_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "source_journals",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    source_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    performed_by_id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_source_journals", x => x.id);
                    table.ForeignKey(
                        name: "fk_source_journals_sources_source_id",
                        column: x => x.source_id,
                        principalTable: "sources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_source_journals_users_performed_by_id",
                        column: x => x.performed_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_dossier_journals_dossier_id",
                table: "dossier_journals",
                column: "dossier_id");

            migrationBuilder.CreateIndex(
                name: "ix_dossier_journals_performed_by_id",
                table: "dossier_journals",
                column: "performed_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_journals_issue_id",
                table: "issue_journals",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_journals_performed_by_id",
                table: "issue_journals",
                column: "performed_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_source_journals_performed_by_id",
                table: "source_journals",
                column: "performed_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_source_journals_source_id",
                table: "source_journals",
                column: "source_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dossier_journals");

            migrationBuilder.DropTable(
                name: "issue_journals");

            migrationBuilder.DropTable(
                name: "source_journals");
        }
    }
}
