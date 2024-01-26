using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class StringResourceEditedTrace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "edited_by_id",
                table: "string_resources",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_edited",
                table: "string_resources",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "ix_string_resources_edited_by_id",
                table: "string_resources",
                column: "edited_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_string_resources_users_edited_by_id",
                table: "string_resources",
                column: "edited_by_id",
                principalTable: "asp_net_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_string_resources_users_edited_by_id",
                table: "string_resources");

            migrationBuilder.DropIndex(
                name: "ix_string_resources_edited_by_id",
                table: "string_resources");

            migrationBuilder.DropColumn(
                name: "edited_by_id",
                table: "string_resources");

            migrationBuilder.DropColumn(
                name: "last_edited",
                table: "string_resources");
        }
    }
}
