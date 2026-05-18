using Microsoft.EntityFrameworkCore;
using HRSystem.Models;

namespace HRSystem.Data
{
    public class HRContext : DbContext
    {
        public HRContext(DbContextOptions<HRContext> options) : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Departments ────────────────────────────────────────
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, Name = "Sales",            Location = "Level 3, Building A" },
                new Department { DepartmentId = 2, Name = "Engineering",      Location = "Level 5, Building B" },
                new Department { DepartmentId = 3, Name = "Human Resources",  Location = "Level 1, Building A" },
                new Department { DepartmentId = 4, Name = "Finance",          Location = "Level 2, Building A" },
                new Department { DepartmentId = 5, Name = "Marketing",        Location = "Level 4, Building B" }
            );

            // ── Employees ──────────────────────────────────────────
            modelBuilder.Entity<Employee>().HasData(
                // Sales
                new Employee { EmployeeId = 1,  FirstName = "Sarah",    LastName = "Mitchell",   Email = "s.mitchell@company.com",   Phone = "07 3001 1001", JobTitle = "Sales Manager",           HireDate = new DateTime(2019, 3, 15),  DepartmentId = 1 },
                new Employee { EmployeeId = 2,  FirstName = "James",    LastName = "Cooper",     Email = "j.cooper@company.com",     Phone = "07 3001 1002", JobTitle = "Sales Representative",    HireDate = new DateTime(2021, 7, 1),   DepartmentId = 1 },
                new Employee { EmployeeId = 3,  FirstName = "Priya",    LastName = "Sharma",     Email = "p.sharma@company.com",     Phone = "07 3001 1003", JobTitle = "Sales Representative",    HireDate = new DateTime(2022, 1, 10),  DepartmentId = 1 },

                // Engineering
                new Employee { EmployeeId = 4,  FirstName = "Vernon",     LastName = "Smith",     Email = "v.smith@company.com",     Phone = "07 3001 2001", JobTitle = "Senior Developer",        HireDate = new DateTime(2018, 6, 20),  DepartmentId = 2 },
                new Employee { EmployeeId = 5,  FirstName = "Emma",     LastName = "Johnson",    Email = "e.johnson@company.com",    Phone = "07 3001 2002", JobTitle = "Developer",               HireDate = new DateTime(2020, 11, 5),  DepartmentId = 2 },
                new Employee { EmployeeId = 6,  FirstName = "Chen",     LastName = "Wei",        Email = "c.wei@company.com",        Phone = "07 3001 2003", JobTitle = "Developer",               HireDate = new DateTime(2023, 2, 14),  DepartmentId = 2 },

                // Human Resources
                new Employee { EmployeeId = 7,  FirstName = "Olivia",   LastName = "Brown",      Email = "o.brown@company.com",      Phone = "07 3001 3001", JobTitle = "HR Manager",              HireDate = new DateTime(2017, 9, 1),   DepartmentId = 3 },
                new Employee { EmployeeId = 8,  FirstName = "Daniel",   LastName = "Smith",      Email = "d.smith@company.com",      Phone = "07 3001 3002", JobTitle = "HR Officer",              HireDate = new DateTime(2021, 4, 18),  DepartmentId = 3 },

                // Finance
                new Employee { EmployeeId = 9,  FirstName = "Emi",    LastName = "Anderson",      Email = "e.anderson@company.com",      Phone = "07 3001 4001", JobTitle = "Finance Manager",         HireDate = new DateTime(2016, 1, 12),  DepartmentId = 4 },
                new Employee { EmployeeId = 10, FirstName = "Tom",      LastName = "Williams",   Email = "t.williams@company.com",   Phone = "07 3001 4002", JobTitle = "Quotation Officer",              HireDate = new DateTime(2022, 8, 22),  DepartmentId = 4 },

                // Marketing
                new Employee { EmployeeId = 11, FirstName = "Megan",    LastName = "Taylor",     Email = "m.taylor@company.com",     Phone = "07 3001 5001", JobTitle = "Marketing Manager",       HireDate = new DateTime(2020, 5, 3),   DepartmentId = 5 },
                new Employee { EmployeeId = 12, FirstName = "Raj",      LastName = "Kumar",      Email = "r.kumar@company.com",      Phone = "07 3001 5002", JobTitle = "Digital Marketing Analyst",HireDate = new DateTime(2023, 6, 19),  DepartmentId = 5 }
            );
        }
    }
}
