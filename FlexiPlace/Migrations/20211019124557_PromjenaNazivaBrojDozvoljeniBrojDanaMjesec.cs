using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class PromjenaNazivaBrojDozvoljeniBrojDanaMjesec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DozvoljeniBrojDanaMjesecu",
                table: "Parametar",
                newName: "DozvoljeniBrojDanaMjesec");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DozvoljeniBrojDanaMjesec",
                table: "Parametar",
                newName: "DozvoljeniBrojDanaMjesecu");
        }
    }
}
