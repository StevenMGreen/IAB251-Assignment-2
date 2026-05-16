using System.Net.Http.Json;

namespace InterportCargo.Web.Services;

/// <summary>
/// Calls the external HR API to validate employee status and role.
/// The base URL is configured via appsettings.json ("HrApi:BaseUrl").
/// </summary>
public class HrApiService(HttpClient httpClient) : IHrApiService
{
    public async Task<HrEmployee?> GetEmployeeByEmailAsync(string email)
    {
        // TODO: update the endpoint path once the dummy HR API is provided
        var response = await httpClient.GetAsync($"api/employees?email={Uri.EscapeDataString(email)}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<HrEmployee>();
    }
}
