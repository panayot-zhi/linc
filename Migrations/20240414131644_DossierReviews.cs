using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class DossierReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dossier_reviews",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    first_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reviewer_id = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dossier_id = table.Column<int>(type: "int", nullable: false),
                    review_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dossier_reviews", x => x.id);
                    table.ForeignKey(
                        name: "fk_dossier_reviews_documents_review_id",
                        column: x => x.review_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dossier_reviews_dossiers_dossier_id",
                        column: x => x.dossier_id,
                        principalTable: "dossiers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dossier_reviews_users_reviewer_id",
                        column: x => x.reviewer_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_dossier_reviews_dossier_id",
                table: "dossier_reviews",
                column: "dossier_id");

            migrationBuilder.CreateIndex(
                name: "ix_dossier_reviews_review_id",
                table: "dossier_reviews",
                column: "review_id");

            migrationBuilder.CreateIndex(
                name: "ix_dossier_reviews_reviewer_id",
                table: "dossier_reviews",
                column: "reviewer_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dossier_reviews");
        }
    }
}
