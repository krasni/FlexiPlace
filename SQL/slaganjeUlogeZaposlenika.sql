/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [Id]
      ,[Name]
      ,[Active]
      ,[RowModifiedAt]
  FROM [BP22T].[dbo].[vKonto_New_UlogeZaposlenika]
  --67

  select * from [HumanResources].[EmployeeRole]

  select * from vViewDjelatnici d
  left join 
	[HumanResources].[EmployeeRole] er on d.IDZaposlenog = er.Id
	where Username like '%borota%'


	select* from 	[HumanResources].[EmployeeRole]


	select 
		*
	from
		 [BP22T].[dbo].[vViewDjelatnici] d
		 left join  [HYPERION].[BP22T].[HumanResources].[EmployeeOrganizationUnit] eou ON d.IDZaposlenog = eou.EmployeeId
		 left join [BP22T].[dbo].[vKonto_New_UlogeZaposlenika] uz on eou.EmployeeRoleId = uz.Id
	where
		(d.OrgID >= 31 OR d.OrgID = 1)	
		and uz.Active = 1
		and (eou.EndDate IS NULL OR GETDATE() <= eou.EndDate)
		--and 	d.Username like '%HANFA\bbasic%' 
		and eou.RowDeleted = 0
		-- 189

		select * from  [BP22T].[dbo].[vViewDjelatnici] d
			where
		(d.OrgID >= 31 OR d.OrgID = 1)	
		-- 190



			select 
		d.username
	from
		 [BP22T].[dbo].[vViewDjelatnici] d
		 left join  [HYPERION].[BP22T].[HumanResources].[EmployeeOrganizationUnit] eou ON d.IDZaposlenog = eou.EmployeeId
		 left join [BP22T].[dbo].[vKonto_New_UlogeZaposlenika] uz on eou.EmployeeRoleId = uz.Id
	where
	(d.OrgID >= 31 OR d.OrgID = 1)	
		and uz.Active = 1
		and (eou.EndDate IS NULL OR GETDATE() <= eou.EndDate)
		--and 	d.Username like '%HANFA\bbasic%' 
		and eou.RowDeleted = 0
		-- 189
	group by d.Username
	having count(*) > 1
		--and 	d.Username like '%borota%' 
		-- 192