using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace linc.Migrations
{
    public partial class ReleaseDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "release_year",
                table: "issues");

            migrationBuilder.AddColumn<DateOnly>(
                name: "release_date",
                table: "issues",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "release_date",
                table: "issues");

            migrationBuilder.AddColumn<int>(
                name: "release_year",
                table: "issues",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
