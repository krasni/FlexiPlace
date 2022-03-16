SELECT
	*
FROM
(
select distinct 
		p.FullName AS ImePrezime,
		p.EMail,
		ou.Title AS OrgJedinicaUKojojOsobaSjedi,
		ou.OrganisationalUnitPath AS OrgJedinicaPutanjaUKojojOsobaSjedi,
		ou.OrganisationalUnitCodePath AS OrgJedinicaSifraUKojojOsobaSjedi,
		--ou.ManagerFullName AS ImePrezimeManagera,
		--rd.Title AS NazivRole,
		ouRole.Title AS OrgJedinicaZaKojuOsobaImaRolu,
		ouRole.OrganisationalUnitPath AS OrgJedinicaPutanjaZaKojuOsobaImaRolu,
		ouRole.OrganisationalUnitCodePath AS OrgJedinicaSifraZaKojuOsobaImaRolu,
		MenaðerOrgJediniceUKojojSjedi = CASE WHEN(p.EntityID = ou.ManagerEntityID) THEN 1 ELSE 0 END,
		MenaðerOrgJediniceUKojojImaRolu = CASE WHEN(p.EntityID = ouRole.ManagerEntityID) THEN 1 ELSE 0 END
	from
		GORGON.eVision_OfficePoint_Content.dbo.persons p
		inner join GORGON.eVision_OfficePoint_Content.dbo.OrganisationalUnits ou on p.OrganisationalUnitEntityID = ou.EntityID
		inner join GORGON.eVision_OfficePoint_Content.dbo.Roles r ON p.EntityID = r.PersonID
		inner join GORGON.eVision_OfficePoint_Content.dbo.RoleDefinitions rd ON r.RoleDefinitionID = rd.Id
		inner join GORGON.eVision_OfficePoint_Content.dbo.OrganisationalUnits ouRole ON r.OrganisationalUnitId = ouRole.EntityID
	where
		p.deleted = 0
		AND
		r.IsActive = 1
		AND
		r.ActiveTo IS NULL
		AND
		r.ActiveFrom >= '2018-10-31'
		and
		(ou.ExternalID >= 31 or (ou.ExternalID is null) or ou.ExternalID = 1)
		and (rd.Title = 'Administrativna osoba' OR rd.Title = 'Odgovorna osoba' OR p.EntityID = ouRole.ManagerEntityID)

	UNION

	SELECT DISTINCT
		M.ImePrezime AS ImePrezime,
		M.EMail,
		M.Title AS OrgJedinicaUKojojOsobaSjedi,
		M.OrganisationalUnitPath AS OrgJedinicaPutanjaUKojojOsobaSjedi,
		M.OrganisationalUnitCodePath AS OrgJedinicaSifraUKojojOsobaSjedi,
		--'Nasljedno jer je menaðer' AS NazivRole,
		OU.Title AS OrgJedinicaZaKojuOsobaImaRolu,
		OU.OrganisationalUnitPath AS OrgJedinicaPutanjaZaKojuOsobaImaRolu,
		OU.OrganisationalUnitCodePath AS OrgJedinicaSifraZaKojuOsobaImaRolu,
		MenaðerOrgJediniceUKojojSjedi = 0,
		MenaðerOrgJediniceUKojojImaRolu = 0
	FROM 
		 GORGON.eVision_OfficePoint_Content.dbo.OrganisationalUnits OU
		INNER JOIN 
		(
			SELECT
				ou.organisationalunitfullcode,
				ou.Title,
				ou.OrganisationalUnitPath,
				ou.OrganisationalUnitCodePath,
				p.FullName AS ImePrezime,
				p.EMail
			FROM
				 GORGON.eVision_OfficePoint_Content.dbo.OrganisationalUnits ou
				INNER JOIN  GORGON.eVision_OfficePoint_Content.dbo.Persons p ON ou.ManagerEntityID = p.EntityID
			WHERE
				len(ou.organisationalunitfullcode) = 9
				AND
				ou.deleted = 0
				AND
				ou.ManagerEntityID IS NOT NULL
				and
				(ou.ExternalID >= 31 or (ou.ExternalID is null) or ou.ExternalID = 1)
		) M
		  ON OU.OrganisationalUnitFullCode LIKE M.OrganisationalUnitFullCode + '-%'
	WHERE
		ou.Deleted = 0
		AND
		LEN(ou.OrganisationalUnitFullCode) = 12
		AND
		(ou.ExternalID >= 31 or (ou.ExternalID is null) or ou.ExternalID = 1)

	UNION
		SELECT DISTINCT
			p.FullName AS ImePrezime,
			p.EMail,
			ou.Title AS OrgJedinicaUKojojOsobaSjedi,
			ou.OrganisationalUnitPath AS OrgJedinicaPutanjaUKojojOsobaSjedi,
			ou.OrganisationalUnitCodePath AS OrgJedinicaSifraUKojojOsobaSjedi,
			--ou.ManagerFullName AS ImePrezimeManagera
			--NULL AS NazivRole,
			NULL AS OrgJedinicaZaKojuOsobaImaRolu,
			NULL AS OrgJedinicaPutanjaZaKojuOsobaImaRolu,
			NULL AS OrgJedinicaSifraZaKojuOsobaImaRolu,
			NULL,
			NULL
		FROM 
			 GORGON.eVision_OfficePoint_Content.dbo.OrganisationalUnits ou 
			INNER JOIN  GORGON.eVision_OfficePoint_Content.dbo.persons p ON ou.ManagerEntityID = p.EntityID
		WHERE 
			ou.EntityID NOT IN
			(SELECT OrganisationalUnitEntityID FROM GORGON.eVision_OfficePoint_Content.dbo.persons)
		AND ou.Deleted = 0
		AND
		(ou.ExternalID >= 31 OR (ou.ExternalID IS NULL) OR ou.ExternalID = 1)
		AND ou.ManagerEntityID IS NOT NULL
)temp
WHERE
	(
	OrgJedinicaSifraZaKojuOsobaImaRolu LIKE '326-01-40%' -- Morana Derenèinoviæ Ruk -- Sektor za superviziju fondova i investicijskih društava
	OR
	OrgJedinicaSifraZaKojuOsobaImaRolu LIKE '326-01-70%' -- Anamarija Stanièiæ -- Sektor za harmonizaciju propisa i meðunarodnu suradnju
	OR
	OrgJedinicaSifraZaKojuOsobaImaRolu LIKE '326-01-50%' -- Ljiljana Mariæ -- Sektor za superviziju osiguranja leasinga i faktoringa
	OR
	OrgJedinicaSifraZaKojuOsobaImaRolu LIKE '326-01-80%'-- Ivana Herceg -- Sektor za sistemske rizike i zaštitu potrošaèa
	OR
	OrgJedinicaSifraZaKojuOsobaImaRolu LIKE '326-01-30-33%' -- Miljenko Cvitanoviæ -- Direkcija za upravljanje ljudskim resursima
	)