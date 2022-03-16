using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class SPRepGetZahtjeviIzbacenaIspravnost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjev_Ispravnost_IspravnostId",
                table: "Zahtjev");

            migrationBuilder.DropTable(
                name: "Ispravnost");

            migrationBuilder.DropIndex(
                name: "IX_Zahtjev_IspravnostId",
                table: "Zahtjev");

            migrationBuilder.DropColumn(
                name: "IspravnostId",
                table: "Zahtjev");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IspravnostId",
                table: "Zahtjev",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Ispravnost",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IspravnostStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ispravnost", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Ispravnost",
                columns: new[] { "Id", "IspravnostStatus" },
                values: new object[] { 1, "Zadovoljava uvjete" });

            migrationBuilder.InsertData(
                table: "Ispravnost",
                columns: new[] { "Id", "IspravnostStatus" },
                values: new object[] { 2, "Ne zadovljava uvjete" });

            migrationBuilder.CreateIndex(
                name: "IX_Zahtjev_IspravnostId",
                table: "Zahtjev",
                column: "IspravnostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Zahtjev_Ispravnost_IspravnostId",
                table: "Zahtjev",
                column: "IspravnostId",
                principalTable: "Ispravnost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
