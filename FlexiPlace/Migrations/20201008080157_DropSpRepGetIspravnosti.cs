using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class DropSpRepGetIspravnosti : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"DROP PROCEDURE [dbo].[spRepGetIspravnosti]
GO
";

			migrationBuilder.Sql(sp);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
