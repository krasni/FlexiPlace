using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class AlterSpGetZahtjevi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"ALTER PROCEDURE [dbo].[spRepGetZahtjevi]
	@Status nvarchar(max),
	@DatumOtvaranjaOd datetime,
	@DatumOtvaranjaDo datetime,
	@DatumOdsustvaOd datetime,
	@DatumOdsustvaDo datetime,
	@OrganizacijaPodnositelj nvarchar(max),
	@OrganizacijaOdobravatelj nvarchar(max),
	@Podnositelj nvarchar(max),
	@Odobravatelj nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
		z.Id,
		ImePrezimePodnositelj,
		OrganizacijskaJedinicaPodnositelj,
		OdobravateljImePrezime,
		s.Naziv as Status,
		DatumOtvaranja,
		DatumOdsustva
	FROM
		Zahtjev z
		LEFT JOIN Status s ON z.StatusId = s.Id
	WHERE
		s.Naziv IN ( SELECT * FROM dbo.Test_Split(@Status))
		AND 
			z.OrganizacijskaJedinicaPodnositelj IN ( SELECT * FROM dbo.Test_Split(@OrganizacijaPodnositelj))
		AND 
			z.OdobravateljNazivOrganizacijskeJedinice IN ( SELECT * FROM dbo.Test_Split(@OrganizacijaOdobravatelj))
		AND
		(
			(@DatumOtvaranjaOd IS NULL OR z.DatumOtvaranja >= @DatumOtvaranjaOd)
			AND
			(@DatumOtvaranjaDo IS NULL OR z.DatumOtvaranja <= @DatumOtvaranjaDo + 1)
		)
		AND 
		(
			(@DatumOdsustvaOd IS NULL OR z.DatumOdsustva >= @DatumOdsustvaOd)
			AND
			(@DatumOdsustvaDo IS NULL OR z.DatumOtvaranja <= @DatumOdsustvaDo)
		)
		AND
		(
			@Podnositelj IS NULL OR z.ImePrezimePodnositelj LIKE '%' + @Podnositelj + '%'
		)
		AND
		(
			@Odobravatelj IS NULL OR z.OdobravateljImePrezime LIKE '%' + @Odobravatelj + '%'
		)
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
