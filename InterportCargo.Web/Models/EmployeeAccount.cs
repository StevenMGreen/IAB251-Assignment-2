namespace InterportCargo.Web.Models;

/// <summary>
/// Stores the local login credential and system role for an employee.
///
/// Responsibility split:
///   - HR API  → confirms the employee exists as an active staff member
///   - This table → stores the hashed key (prototype auth) and the
///     role granted within the Quotation System (e.g. QuotationOfficer)
///
/// Seed at least one record for testing. Use an email that exists in the
/// HR system (see API_SPECIFICATION.md sample data for valid emails).
/// Example seed:
///   Email   = "j.cooper@company.com"
///   KeyHash = BCrypt.HashPassword("Test1234!")
///   Role    = "QuotationOfficer"
/// </summary>
public class EmployeeAccount
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;

    /// <summary>
    /// Role within the Quotation System. Must be "QuotationOfficer"
    /// to gain access to quotation management functions.
    /// </summary>
    public string Role { get; set; } = "QuotationOfficer";
}
