using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using InterportCargo.Web.Data;
using InterportCargo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InterportCargo.Web.Pages.Customers;

[Authorize(Roles = "Customer")]
public class RequestQuotationModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public QuotationRequestInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var request = new QuotationRequest
        {
            RequestReference = $"REQ-{DateTime.UtcNow:yyyyMMddHHmmss}",
            CustomerId = customerId,
            Source = Input.Source,
            Destination = Input.Destination,
            NumberOfContainers = Input.NumberOfContainers,
            PackageNature = Input.PackageNature,
            JobNature = Input.JobNature,
            RequiresPacking = Input.RequiresPacking,
            RequiresQuarantine = Input.RequiresQuarantine
        };

        db.QuotationRequests.Add(request);
        await db.SaveChangesAsync();

        return RedirectToPage("/Customers/Dashboard");
    }
}

public class QuotationRequestInput
{
    [Required]
    public string Source { get; set; } = string.Empty;

    [Required]
    public string Destination { get; set; } = string.Empty;

    [Required, Range(1, int.MaxValue, ErrorMessage = "At least 1 container is required.")]
    [Display(Name = "Number of Containers")]
    public int NumberOfContainers { get; set; }

    [Required, Display(Name = "Nature of Package")]
    public string PackageNature { get; set; } = string.Empty;

    [Required, Display(Name = "Import / Export")]
    public JobNature JobNature { get; set; }

    [Display(Name = "Requires Packing")]
    public bool RequiresPacking { get; set; }

    [Display(Name = "Requires Quarantine")]
    public bool RequiresQuarantine { get; set; }
}
