using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class DodanaStoraZaDjelatnike : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE [dbo].[spGetDjelatnici]

            AS
            BEGIN
	
            SET NOCOUNT ON;

			SELECT
				d.Prezime + ' ' + d.Ime AS ImePrezime
			FROM
				 BP22T.dbo.vViewDjelatnici d 
			WHERE
				(d.OrgID >= 31 OR d.OrgID = 1)
			ORDER BY
				d.Prezime,
				d.Ime

			END
";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
