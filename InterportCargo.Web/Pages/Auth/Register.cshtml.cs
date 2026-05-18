using System.ComponentModel.DataAnnotations;
using InterportCargo.Web.Data;
using InterportCargo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InterportCargo.Web.Pages.Auth;

public class RegisterModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        bool emailTaken = await db.Customers.AnyAsync(c => c.Email == Input.Email);
        if (emailTaken)
        {
            ModelState.AddModelError(nameof(Input.Email), "An account with this email already exists.");
            return Page();
        }

        var customer = new Customer
        {
            FirstName = Input.FirstName,
            FamilyName = Input.FamilyName,
            Email = Input.Email,
            Phone = Input.Phone,
            CompanyName = Input.CompanyName,
            Address = Input.Address,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password)
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        return RedirectToPage("/Auth/CustomerLogin", new { registered = true });
    }
}

public class RegisterInput
{
    [Required, Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, Display(Name = "Family Name")]
    public string FamilyName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, RegularExpression(@"^[0-9\s\+\-\(\)]{6,20}$", ErrorMessage = "Please enter a valid phone number.")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "Company Name (optional)")]
    public string? CompanyName { get; set; }

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required, MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
