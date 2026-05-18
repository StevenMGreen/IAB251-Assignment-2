namespace InterportCargo.Web.Services;

/// <summary>
/// Matches the summary employee object returned by GET /api/employees.
/// Property names match the HR API JSON response exactly.
/// </summary>
public record HrEmployeeSummary(
    int EmployeeId,
    string FirstName,
    string LastName,
    string Email,
    string JobTitle,
    string DepartmentName
);

public interface IHrApiService
{
    /// <summary>
    /// Looks up an employee by email address in the HR system.
    /// Returns null if no matching employee is found.
    /// A non-null result means the employee exists and is considered active.
    /// </summary>
    Task<HrEmployeeSummary?> GetEmployeeByEmailAsync(string email);
}
