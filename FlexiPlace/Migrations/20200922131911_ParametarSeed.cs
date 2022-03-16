using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class ParametarSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Parametar",
                columns: new[] { "Id", "DozvoljeniBrojDanaMjesecu", "DozvoljeniBrojaDanaOdobrenje" },
                values: new object[] { 1, 10, 10 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parametar",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
