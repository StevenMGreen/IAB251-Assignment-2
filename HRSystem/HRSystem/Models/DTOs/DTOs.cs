namespace HRSystem.Models.DTOs
{
    // ── Employee DTOs ──────────────────────────────────────────────

    /// <summary>
    /// Summary view of an employee (used in list endpoints).
    /// </summary>
    public class EmployeeSummaryDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Full detail view of an employee (used in single-employee endpoint).
    /// </summary>
    public class EmployeeDetailDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }

    // ── Department DTOs ────────────────────────────────────────────

    /// <summary>
    /// Department with its list of employees.
    /// </summary>
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<EmployeeSummaryDto> Employees { get; set; } = new();
    }
}
