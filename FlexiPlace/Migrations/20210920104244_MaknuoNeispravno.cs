using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class MaknuoNeispravno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "NeispravnostKomentar",
                table: "Zahtjev");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 5,
                column: "Naziv",
                value: "Obrisan");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 6,
                column: "Naziv",
                value: "Otkazan");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NeispravnostKomentar",
                table: "Zahtjev",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 5,
                column: "Naziv",
                value: "Neispravan");

            migrationBuilder.UpdateData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 6,
                column: "Naziv",
                value: "Obrisan");

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Naziv" },
                values: new object[] { 7, "Otkazan" });
        }
    }
}
