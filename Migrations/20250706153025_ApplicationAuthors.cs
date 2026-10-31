using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class ApplicationAuthors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    first_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    language_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "varchar(127)", maxLength: 127, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    source_id = table.Column<int>(type: "int", nullable: true),
                    dossier_id = table.Column<int>(type: "int", nullable: true),
                    last_updated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_authors", x => x.id);
                    table.ForeignKey(
                        name: "fk_authors_dossiers_dossier_id",
                        column: x => x.dossier_id,
                        principalTable: "dossiers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_authors_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_authors_sources_source_id",
                        column: x => x.source_id,
                        principalTable: "sources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_authors_users_user_id",
                        column: x => x.user_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // TODO: Note that a curious case arises when both the source_id AND the dossier_id are set to NULL
            // for an author - the entry becomes orphaned and needs to be cleaned up
            // maybe through a UI list that will highlight this lack of relationship

            migrationBuilder.CreateIndex(
                name: "ix_authors_dossier_id",
                table: "authors",
                column: "dossier_id");

            migrationBuilder.CreateIndex(
                name: "ix_authors_email",
                table: "authors",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_authors_first_name",
                table: "authors",
                column: "first_name");

            migrationBuilder.CreateIndex(
                name: "ix_authors_language_id",
                table: "authors",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "ix_authors_last_name",
                table: "authors",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "ix_authors_source_id",
                table: "authors",
                column: "source_id");

            migrationBuilder.CreateIndex(
                name: "ix_authors_user_id",
                table: "authors",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authors");
        }
    }
}
