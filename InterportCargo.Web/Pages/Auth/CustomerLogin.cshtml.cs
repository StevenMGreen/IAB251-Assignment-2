using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using InterportCargo.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InterportCargo.Web.Pages.Auth;

public class CustomerLoginModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public bool JustRegistered { get; set; }

    public void OnGet(bool registered = false)
    {
        JustRegistered = registered;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.Email == Input.Email);

        bool credentialsValid = customer is not null
            && BCrypt.Net.BCrypt.Verify(Input.Password, customer.PasswordHash);

        if (!credentialsValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customer!.Id.ToString()),
            new(ClaimTypes.Name, $"{customer.FirstName} {customer.FamilyName}"),
            new(ClaimTypes.Email, customer.Email),
            new(ClaimTypes.Role, "Customer")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToPage("/Customers/Dashboard");
    }
}

public class LoginInput
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
