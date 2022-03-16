using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class DodanaStoraZaOrganizacijskeJedinice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE [dbo].[spGetOrganizacijskeJedinice]

            AS
            BEGIN
	
            SET NOCOUNT ON;

			SELECT
				OrgNaziv
			FROM
				 BP22T.dbo.vViewOrganizacijskaStruktura os
			WHERE
				(os.OrgID >= 31 OR os.OrgID = 1)
            ORDER BY
                OrgNaziv
			END";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
