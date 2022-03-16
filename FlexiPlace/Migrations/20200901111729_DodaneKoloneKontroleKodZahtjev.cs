using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class DodaneKoloneKontroleKodZahtjev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Zahtjev",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Zahtjev",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifedBy",
                table: "Zahtjev",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Zahtjev",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Zahtjev");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Zahtjev");

            migrationBuilder.DropColumn(
                name: "ModifedBy",
                table: "Zahtjev");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Zahtjev");
        }
    }
}
