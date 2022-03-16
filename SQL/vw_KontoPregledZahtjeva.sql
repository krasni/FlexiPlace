ALTER VIEW vw_KontoPregledZahtjeva
AS 
	SELECT
		 z.id
		,PersonId as SifraDjelatnika
		,[DatumOdsustva]
		,[CreatedDate]
		,[ModifiedDate]
		,[Naziv] AS Status
	FROM
		[dbo].[Zahtjev] z
		INNER JOIN [dbo].[Status] s ON z.StatusId = s.id
		INNER JOIN BP22T.HumanResources.Employee e ON z.podnositelj COLLATE DATABASE_DEFAULT = e.ADName COLLATE DATABASE_DEFAULT
	WHERE
		s.id IN (2, 7)
				