/****** Object:  StoredProcedure [dbo].[spFetchNadredeni]    Script Date: 7.7.2020. 10:11:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[spFetchNadredeni]
	@EmployeeADName nvarchar(128)

AS
BEGIN
	
SET NOCOUNT ON;

;WITH CTE AS
(
SELECT HE.ADName
	, O.Name
	, O.OrganizationUnitParentId
	, E.Id AS UserCheck
FROM BP2.HumanResources.Employee as E
INNER JOIN BP2.HumanResources.EmployeeOrganizationUnit AS EO
	ON EO.EmployeeId = E.Id
INNER JOIN BP2.HumanResources.OrganizationUnit AS O
	ON O.Id = EO.OrganizationUnitId
INNER JOIN BP2.HumanResources.Employee AS HE
	ON HE.Id = O.OrganizationUnitHeadId
WHERE E.ADName = @EmployeeADName
	AND EO.EndDate IS NULL

UNION ALL

SELECT HE.ADName
	, O.Name
	, O.OrganizationUnitParentId
	, HE.Id
FROM CTE
INNER JOIN BP2.HumanResources.OrganizationUnit AS O
	ON O.Id = CTE.OrganizationUnitParentId
INNER JOIN BP2.HumanResources.Employee AS HE
	ON HE.Id = O.OrganizationUnitHeadId
)

SELECT ADName
FROM CTE
WHERE ADName <> @EmployeeADName

END
GO


