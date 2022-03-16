using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class Add_OrganizacijskaJedinicaPutanjaPodnositelj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizacijskaJedinicaPutanjaPodnositelj",
                table: "Zahtjev",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizacijskaJedinicaPutanjaPodnositelj",
                table: "Zahtjev");
        }
    }
}
