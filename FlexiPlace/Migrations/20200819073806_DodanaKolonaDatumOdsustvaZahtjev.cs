﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class DodanaKolonaDatumOdsustvaZahtjev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DatumOdsustva",
                table: "Zahtjev",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatumOdsustva",
                table: "Zahtjev");
        }
    }
}
