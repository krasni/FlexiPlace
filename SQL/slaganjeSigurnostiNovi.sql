declare @EmployeeADName nvarchar(128)
set @employeeadname = 'HANFA\iborota'

;WITH CTE AS
(
	SELECT 
		@employeeadname AS EmployeeADName,
		manager.Username AS OdobravateljADName,
		manager.Prezime + ' ' + manager.Ime AS OdobravateljImePrezime,
		os.OrgNaziv AS OdobravateljNazivOrganizacijskeJedinice,
		ouh.Putanja AS OdobravateljPutanja,
		CAST(parentOrg.OrgID AS INT) AS OrganisationUnitParentId,
		d.IDZaposlenog,
		ouh.lvl
	FROM	
		BP2.dbo.vViewDjelatnici d 
		INNER JOIN BP2.dbo.vViewOrganizacijskaStruktura os on d.OrgID = os.OrgID
		INNER JOIN BP2.dbo.vViewOrganizacijskaStruktura parentOrg on os.OrgParentID = parentOrg.OrgID
		INNER JOIN BP2.dbo.vViewOrganisationUnitHierarchy ouh ON os.OrgID = ouh.ID
		LEFT JOIN BP2.dbo.vViewDjelatnici manager ON os.ManagerID = manager.IDZaposlenog
	WHERE
		d.Username = @employeeadname
		AND
		(d.OrgID >= 31 OR d.OrgID = 1)	

	UNION ALL

	SELECT
		@employeeadname AS EmployeeADName,
		manager.Username AS OdobravateljADName,
		manager.Prezime + ' ' + manager.Ime AS OdobravateljImePrezime,
		os.OrgNaziv AS OdobravateljNazivOrganizacijskeJedinice,
		ouh.Putanja AS OdobravateljPutanja,
		os.OrgParentID AS OrganisationUnitParentId,
		manager.IDZaposlenog,
		ouh.lvl
	FROM 
		CTE
		INNER JOIN BP2.dbo.vViewOrganizacijskaStruktura os on CTE.OrganisationUnitParentId = os.OrgID
		INNER JOIN BP2.dbo.vViewOrganisationUnitHierarchy ouh ON CTE.OrganisationUnitParentId = ouh.ID
		INNER JOIN BP2.dbo.vViewDjelatnici manager ON os.ManagerID = manager.IDZaposlenog
	WHERE
		(os.OrgID >= 31 OR os.OrgID = 1)	
)

SELECT 
	d.Username,
	d.Prezime + ' ' + d.Ime AS ImePrezime,
	CASE 
		WHEN EXISTS (SELECT * FROM Admin WHERE d.Username = Admin.LoginName) THEN 'Admin'
		WHEN ((os.ManagerID IS NULL OR os.ManagerID = d.IDZaposlenog) AND lvl <> 4) THEN 'Voditelj' 
		ELSE 'Korisnik' END AS Uloga,
	os.OrgNaziv AS NazivOrganizacijskeJedinice,
	ouh.lvl,
	ouh.Putanja,
	Odobravatelj.OdobravateljADName,
	ISNULL(Odobravatelj.OdobravateljImePrezime, 'Èlan Upravnog vijeæa') AS OdobravateljImePrezime,
	ISNULL(Odobravatelj.OdobravateljNazivOrganizacijskeJedinice, 'Upravno vijeæe Agencije') AS OdobravateljNazivOrganizacijskeJedinice,
	ISNULL(Odobravatelj.OdobravateljPutanja, 'Upravno vijeæe Agencije') AS OdobravateljPutanja
FROM	
	BP2.dbo.vViewDjelatnici d 
	INNER JOIN BP2.dbo.vViewOrganizacijskaStruktura os on d.OrgID = os.OrgID
	INNER JOIN BP2.dbo.vViewOrganizacijskaStruktura parentOrg on os.OrgParentID = parentOrg.OrgID
	INNER JOIN BP2.dbo.vViewOrganisationUnitHierarchy ouh ON os.OrgID = ouh.ID
	LEFT JOIN BP2.dbo.vViewDjelatnici manager ON os.ManagerID = manager.IDZaposlenog
	LEFT JOIN 
	(
		SELECT
			@employeeadname AS EmployeeADName,
			OdobravateljADName,
			OdobravateljImePrezime,
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
-- 194 sa upravom, level 1

-- ajmo naæi odobravatelja, za null mogu staviti uprava, nesmije biti level 4 !!!
--select * from vViewDjelatnici where IDZaposlenog = 259
