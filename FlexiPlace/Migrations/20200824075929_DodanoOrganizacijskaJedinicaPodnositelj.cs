using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class DodanoOrganizacijskaJedinicaPodnositelj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizacijskaJedinicaPodnositelj",
                table: "Zahtjev",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizacijskaJedinicaPodnositelj",
                table: "Zahtjev");
        }
    }
}
