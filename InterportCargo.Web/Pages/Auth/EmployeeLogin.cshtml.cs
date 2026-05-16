using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using InterportCargo.Web.Data;
using InterportCargo.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InterportCargo.Web.Pages.Auth;

/// <summary>
/// T3: Employee login.
/// Step 1 — validate local credential (email + hashed key).
/// Step 2 — call HR API to confirm active employee with QuotationOfficer role.
/// A generic error is shown on any failure to avoid leaking which step failed.
/// </summary>
public class EmployeeLoginModel(AppDbContext db, IHrApiService hrApiService) : PageModel
{
    [BindProperty]
    public EmployeeLoginInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Step 1: validate local credential
        var account = await db.EmployeeAccounts
            .FirstOrDefaultAsync(e => e.Email == Input.Email);

        bool localCredentialValid = account is not null
            && BCrypt.Net.BCrypt.Verify(Input.EmployeeKey, account.KeyHash);

        if (!localCredentialValid)
        {
            ModelState.AddModelError(string.Empty, "Login failed. Please check your credentials.");
            return Page();
        }

        // Step 2: validate via HR API
        var hrEmployee = await hrApiService.GetEmployeeByEmailAsync(Input.Email);

        bool hrValid = hrEmployee is { IsActive: true, Role: "QuotationOfficer" };

        if (!hrValid)
        {
            ModelState.AddModelError(string.Empty, "Login failed. Please check your credentials.");
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, hrEmployee!.FullName),
            new(ClaimTypes.Email, hrEmployee.Email),
            new(ClaimTypes.Role, "QuotationOfficer")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToPage("/Employee/Quotations");
    }
}

public class EmployeeLoginInput
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, Display(Name = "Employee Key")]
    public string EmployeeKey { get; set; } = string.Empty;
}
