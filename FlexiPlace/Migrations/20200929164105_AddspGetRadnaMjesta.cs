using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class AddspGetRadnaMjesta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"CREATE PROCEDURE [dbo].[spGetRadnaMjesta]

            AS
            BEGIN
	
            SET NOCOUNT ON;

select 
	name 
from 
	BP22T.dbo.[vKonto_New_UlogeZaposlenika]
where
	active = 1
order by
	name
	
END

GO
";

			migrationBuilder.Sql(sp);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
