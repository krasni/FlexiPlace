using Microsoft.EntityFrameworkCore.Migrations;

namespace FlexiPlace.Migrations
{
    public partial class Mijenjamstoruopet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"ALTER PROCEDURE [dbo].[spGetUserDataBP2]
	            @EmployeeADName nvarchar(128)

            AS
            BEGIN
	
            SET NOCOUNT ON;


          ;WITH CTE AS
(
	SELECT 
		@employeeadname AS EmployeeADName,
		manager.Username AS OdobravateljADName,
		manager.Prezime + ' ' + manager.Ime AS OdobravateljImePrezime,
		manager.Email AS OdobravateljEmail,
		os.OrgNaziv AS OdobravateljNazivOrganizacijskeJedinice,
		ouh.Putanja AS OdobravateljPutanja,
		CAST(parentOrg.OrgID AS INT) AS OrganisationUnitParentId,
		d.IDZaposlenog,
		ouh.lvl
	FROM	
		BP22T.dbo.vViewDjelatnici d 
		INNER JOIN BP22T.dbo.vViewOrganizacijskaStruktura os on d.OrgID = os.OrgID
		INNER JOIN BP22T.dbo.vViewOrganizacijskaStruktura parentOrg on os.OrgParentID = parentOrg.OrgID
		INNER JOIN BP22T.dbo.vViewOrganisationUnitHierarchy ouh ON os.OrgID = ouh.ID
		LEFT JOIN BP22T.dbo.vViewDjelatnici manager ON os.ManagerID = manager.IDZaposlenog
	WHERE
		d.Username = @employeeadname
		AND
		(d.OrgID >= 31 OR d.OrgID = 1)	

	UNION ALL

	SELECT
		@employeeadname AS EmployeeADName,
		manager.Username AS OdobravateljADName,
		manager.Prezime + ' ' + manager.Ime AS OdobravateljImePrezime,
		manager.Email AS OdobravateljEmail,
		os.OrgNaziv AS OdobravateljNazivOrganizacijskeJedinice,
		ouh.Putanja AS OdobravateljPutanja,
		os.OrgParentID AS OrganisationUnitParentId,
		manager.IDZaposlenog,
		ouh.lvl
	FROM 
		CTE
		INNER JOIN BP22T.dbo.vViewOrganizacijskaStruktura os on CTE.OrganisationUnitParentId = os.OrgID
		INNER JOIN BP22T.dbo.vViewOrganisationUnitHierarchy ouh ON CTE.OrganisationUnitParentId = ouh.ID
		INNER JOIN BP22T.dbo.vViewDjelatnici manager ON os.ManagerID = manager.IDZaposlenog
	WHERE
		(os.OrgID >= 31 OR os.OrgID = 1)	
)

SELECT 
	d.Username,
	d.Prezime + ' ' + d.Ime AS ImePrezime,
	d.Email,
	CASE 
		WHEN EXISTS (SELECT * FROM Admin WHERE d.Username COLLATE DATABASE_DEFAULT = Admin.LoginName COLLATE DATABASE_DEFAULT) THEN 'Admin'
		WHEN ((os.ManagerID IS NULL OR os.ManagerID = d.IDZaposlenog) AND lvl <> 4) THEN 'Voditelj' 
		ELSE 'Korisnik' END AS Uloga,
	os.OrgNaziv AS NazivOrganizacijskeJedinice,
	ouh.lvl,
	ouh.Putanja,
	Odobravatelj.OdobravateljADName,
	ISNULL(Odobravatelj.OdobravateljImePrezime, 'Član Uprave') AS OdobravateljImePrezime,
	ISNULL(Odobravatelj.OdobravateljEmail, 'prezentator@hanfa.hr') AS OdobravateljEmail,
	ISNULL(Odobravatelj.OdobravateljNazivOrganizacijskeJedinice, 'Upravno vijeće Agencije') AS OdobravateljNazivOrganizacijskeJedinice,
	ISNULL(Odobravatelj.OdobravateljPutanja, 'Upravno vijeće Agencije') AS OdobravateljPutanja
FROM	
	BP22T.dbo.vViewDjelatnici d 
	INNER JOIN BP22T.dbo.vViewOrganizacijskaStruktura os on d.OrgID = os.OrgID
	INNER JOIN BP22T.dbo.vViewOrganizacijskaStruktura parentOrg on os.OrgParentID = parentOrg.OrgID
	INNER JOIN BP22T.dbo.vViewOrganisationUnitHierarchy ouh ON os.OrgID = ouh.ID
	LEFT JOIN BP22T.dbo.vViewDjelatnici manager ON os.ManagerID = manager.IDZaposlenog
	LEFT JOIN 
	(
		SELECT
			@employeeadname AS EmployeeADName,
			OdobravateljADName,
			OdobravateljImePrezime,
			OdobravateljEmail,
			OdobravateljNazivOrganizacijskeJedinice,
			OdobravateljPutanja
		FROM
			(
				SELECT 
					*,
					ROW_NUMBER() OVER(ORDER BY lvl DESC) [row_number]
				FROM 
					CTE
				WHERE
					@EmployeeADName <> OdobravateljADName
					AND
					lvl < 4
			)temp
		WHERE
			row_number = 1
	)Odobravatelj ON d.Username = Odobravatelj.EmployeeADName
WHERE
	(d.OrgID >= 31 OR d.OrgID = 1)	
	AND 
	d.Username = @EmployeeADName

END";

			migrationBuilder.Sql(sp);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
