namespace InterportCargo.Web.Models;

/// <summary>
/// Stores the local login credential for an employee.
/// The HR API is the source of truth for employment status and role —
/// this table only holds the hashed key used for prototype authentication.
/// </summary>
public class EmployeeAccount
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
}
