CREATE PROCEDURE [dbo].[spGetUserDataBP2]
	@EmployeeADName nvarchar(128)

AS
BEGIN
	
SET NOCOUNT ON;


SELECT 
	d.Prezime + ' ' + d.Ime AS ImePrezime,
	ou.Name AS NazivOrganizacijskeJedinice
FROM 
	vViewDjelatnici d
	INNER JOIN bp2.HumanResources.OrganizationUnit ou on d.OrgID = ou.id
WHERE 
	d.Username = @EmployeeADName

END