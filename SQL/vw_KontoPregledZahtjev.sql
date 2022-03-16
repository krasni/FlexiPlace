USE [FlexiPlaceDB]
GO

/****** Object:  View [dbo].[vw_KontoPregledZahtjeva]    Script Date: 25.10.2021. 11:44:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[vw_KontoPregledZahtjeva]
AS 
	SELECT
		 z.id
		,e.PersonId as SifraDjelatnika
		,o.PersonId as SifraDjelatnikaOdobrioStornirao
		,[DatumOdsustva]
		,[VrijemeOdsustvaDo]
		,[VrijemeOdsustvaOd]
		,[CreatedDate]
		,[ModifiedDate]
		,[Naziv] AS Status
	FROM
		[dbo].[Zahtjev] z
		LEFT JOIN [dbo].[Status] s ON z.StatusId = s.id
		LEFT JOIN BP22T.HumanResources.Employee e ON z.podnositelj COLLATE DATABASE_DEFAULT = e.ADName COLLATE DATABASE_DEFAULT
		LEFT JOIN BP22T.HumanResources.Employee o ON z.OdobravateljADName COLLATE DATABASE_DEFAULT = o.ADName COLLATE DATABASE_DEFAULT
	WHERE
		s.id IN (2, 7)
				
GO


