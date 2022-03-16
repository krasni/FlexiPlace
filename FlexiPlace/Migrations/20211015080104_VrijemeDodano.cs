using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class VrijemeDodano : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VrijemeOdsustvaDo",
                table: "Zahtjev",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "VrijemeOdsustvaOd",
                table: "Zahtjev",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VrijemeOdsustvaDo",
                table: "Zahtjev");

            migrationBuilder.DropColumn(
                name: "VrijemeOdsustvaOd",
                table: "Zahtjev");
        }
    }
}
