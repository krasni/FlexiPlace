using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class DodaoParKolonaZahtjev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Odobravatelj",
                table: "Zahtjev",
                newName: "OdobravateljNazivOrganizacijskeJedinice");

            migrationBuilder.AddColumn<string>(
                name: "OdobravateljADName",
                table: "Zahtjev",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OdobravateljImePrezime",
                table: "Zahtjev",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OdobravateljADName",
                table: "Zahtjev");

            migrationBuilder.DropColumn(
                name: "OdobravateljImePrezime",
                table: "Zahtjev");

            migrationBuilder.RenameColumn(
                name: "OdobravateljNazivOrganizacijskeJedinice",
                table: "Zahtjev",
                newName: "Odobravatelj");
        }
    }
}
