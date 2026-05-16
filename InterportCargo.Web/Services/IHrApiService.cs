namespace InterportCargo.Web.Services;

public record HrEmployee(
    string Email,
    string FullName,
    string Role,
    bool IsActive
);

public interface IHrApiService
{
    /// <summary>
    /// Looks up an employee by email in the HR system.
    /// Returns null if not found.
    /// </summary>
    Task<HrEmployee?> GetEmployeeByEmailAsync(string email);
}
