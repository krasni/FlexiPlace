;WITH CTE AS
(
SELECT HE.ADName
	, O.Name
	, O.OrganizationUnitParentId
	, E.Id AS UserID
FROM BP2.HumanResources.Employee as E
INNER JOIN BP2.HumanResources.EmployeeOrganizationUnit AS EO
	ON EO.EmployeeId = E.Id
INNER JOIN BP2.HumanResources.OrganizationUnit AS O
	ON O.Id = EO.OrganizationUnitId
INNER JOIN BP2.HumanResources.Employee AS HE
	ON HE.Id = O.OrganizationUnitHeadId
WHERE E.ADName = 'hanfa\iborota'
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

SELECT *
FROM CTE
--WHERE ADName <> @EmployeeADName

----------------------------------------------------------------------------
--- slažemo -----

SELECT
	*
FROM 
	BP2.HumanResources.Employee as E
	INNER JOIN BP2.HumanResources.EmployeeOrganizationUnit AS EO
	ON EO.EmployeeId = E.Id
	INNER JOIN BP2.HumanResources.OrganizationUnit AS O
	ON O.Id = EO.OrganizationUnitId
	INNER JOIN BP2.HumanResources.Employee AS HE
	ON HE.Id = O.OrganizationUnitHeadId
WHERE
	E.ADName like 'HANFA\mkriznjak'
	AND
	E.RowDeleted = 0
	AND
	EO.EndDate IS NULL
	AND
	EO.RowDeleted = 0
	AND
	HE.RowDeleted = 0


-------------------------------------------------------------------------------------------------

SELECT
	e.ADName
FROM 
	BP2.HumanResources.Employee as E
	INNER JOIN BP2.HumanResources.EmployeeOrganizationUnit AS EO
	ON EO.EmployeeId = E.Id
WHERE
	ADName like 'HANFA\iborota'
	AND
	EndDate IS NULL
group by e.adname
having count(*) > 1
-- super nema

select
	adname
from
	BP2.HumanResources.Employee as E
where
	RowDeleted = 0
group by
	adname
having(count(*)) > 1
-- odlièno, treba staviti rowdeleted = 0


select 
	*
from 
	BP2.HumanResources.OrganizationUnit
where
	RowDeleted = 0


select * from BP2.HumanResources.Employee