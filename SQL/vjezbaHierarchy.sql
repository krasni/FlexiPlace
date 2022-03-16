declare @EmployeeADName nvarchar(128)
set @employeeadname = 'hanfa\iborota'


;WITH CTE AS
(
SELECT HE.ADName
	, O.Name
	, O.OrganizationUnitParentId
	, E.Id AS UserID
FROM RESOLUTION.BP2.HumanResources.Employee as E
INNER JOIN RESOLUTION.BP2.HumanResources.EmployeeOrganizationUnit AS EO
	ON EO.EmployeeId = E.Id
INNER JOIN RESOLUTION.BP2.HumanResources.OrganizationUnit AS O
	ON O.Id = EO.OrganizationUnitId
INNER JOIN RESOLUTION.BP2.HumanResources.Employee AS HE
	ON HE.Id = O.OrganizationUnitHeadId
WHERE E.ADName = @EmployeeADName
	AND EO.EndDate IS NULL

UNION ALL

SELECT HE.ADName
	, O.Name
	, O.OrganizationUnitParentId
	, HE.Id
FROM CTE
INNER JOIN RESOLUTION.BP2.HumanResources.OrganizationUnit AS O
	ON O.Id = CTE.OrganizationUnitParentId
INNER JOIN RESOLUTION.BP2.HumanResources.Employee AS HE
	ON HE.Id = O.OrganizationUnitHeadId
)

SELECT *
FROM CTE
WHERE ADName <> @EmployeeADName

--select * from 
--RESOLUTION.BP2.HumanResources.Employee as E
--INNER JOIN RESOLUTION.BP2.HumanResources.EmployeeOrganizationUnit AS EO
--	ON EO.EmployeeId = E.Id
--INNER JOIN RESOLUTION.BP2.HumanResources.OrganizationUnit AS O
--	ON O.Id = EO.OrganizationUnitId
--INNER JOIN RESOLUTION.BP2.HumanResources.Employee AS HE
--	ON HE.Id = O.OrganizationUnitHeadId
--where e.adname like '%borota%'

--select * from  RESOLUTION.BP2.HumanResources.OrganizationUnit