namespace EmployeesWebService.Extensions
{     
    public static class Employees
    {
        // Returns the number of unique employees that have names that match
        // one of the namesToSearch.
        public static int CountEmployeesByName(string[] namesToSearch)
        {
            return 0;
            //using (ApplicationEntities context = new ApplicationEntities())
            //{
            //    var employeeList = context.employees.Batch.Where(x => namesToSearch.Contains(x.namesToSearch, StringComparison.OrdinalIgnoreCase)).ToList();

            //    return employeeList.Count();
            //}
        }
    }
}
/*  Database Table Creation Script 
 *  -----------------------------------
 *  CREATE TABLE [dbo].[Employee](
	[EmployeeID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,	
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)
 WITH (PAD_INDEX = OFF, 
 STATISTICS_NORECOMPUTE = OFF, 
 IGNORE_DUP_KEY = OFF, 
 ALLOW_ROW_LOCKS = ON, 
 ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
*/