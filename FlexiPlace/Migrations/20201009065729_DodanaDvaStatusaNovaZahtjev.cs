using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class DodanaDvaStatusaNovaZahtjev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Naziv" },
                values: new object[] { 6, "Obrisan" });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Naziv" },
                values: new object[] { 7, "Otkazan" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
