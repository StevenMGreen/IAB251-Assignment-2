using System.Net.Http.Json;

namespace InterportCargo.Web.Services;

/// <summary>
/// Calls the external HR API to validate employee existence.
/// Base URL is configured via appsettings.json ("HrApi:BaseUrl").
///
/// The HR API has no email search endpoint, so we retrieve all employees
/// and filter by email in memory. This is acceptable for a prototype with
/// a small employee dataset.
/// </summary>
public class HrApiService(HttpClient httpClient) : IHrApiService
{
    public async Task<HrEmployeeSummary?> GetEmployeeByEmailAsync(string email)
    {
        var employees = await httpClient
            .GetFromJsonAsync<List<HrEmployeeSummary>>("api/employees");

        if (employees is null)
            return null;

        return employees.FirstOrDefault(e =>
            string.Equals(e.Email, email, StringComparison.OrdinalIgnoreCase));
    }
}
