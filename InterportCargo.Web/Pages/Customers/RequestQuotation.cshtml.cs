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
            GoodsType = Input.GoodsType,
            PackageWidth = Input.PackageWidth,
            PackageHeight = Input.PackageHeight,
            JobNature = Input.JobNature,
            RequiresPacking = Input.RequiresPacking,
            RequiresQuarantine = Input.RequiresQuarantine,
            QuarantineOrigin = Input.RequiresQuarantine ? Input.QuarantineOrigin : null,
            QuarantineTreatmentHistory = Input.RequiresQuarantine ? Input.QuarantineTreatmentHistory : null,
            RequiresFumigation = Input.RequiresFumigation,
            FumigationPestThreats = Input.RequiresFumigation ? Input.FumigationPestThreats : null
        };

        db.QuotationRequests.Add(request);
        await db.SaveChangesAsync();

        return RedirectToPage("/Customers/Dashboard", new { submitted = true });
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

    // Nature of package
    [Required, Display(Name = "Goods Type")]
    public string GoodsType { get; set; } = string.Empty;

    [Required, Range(0.01, double.MaxValue, ErrorMessage = "Width must be greater than 0.")]
    [Display(Name = "Width (m)")]
    public decimal PackageWidth { get; set; }

    [Required, Range(0.01, double.MaxValue, ErrorMessage = "Height must be greater than 0.")]
    [Display(Name = "Height (m)")]
    public decimal PackageHeight { get; set; }

    // Nature of job
    [Required, Display(Name = "Import / Export")]
    public JobNature JobNature { get; set; }

    [Display(Name = "Packing / Unpacking Required")]
    public bool RequiresPacking { get; set; }

    // Quarantine
    [Display(Name = "Quarantine Required")]
    public bool RequiresQuarantine { get; set; }

    [Display(Name = "Quarantine Origin")]
    public string? QuarantineOrigin { get; set; }

    [Display(Name = "Treatment History")]
    public string? QuarantineTreatmentHistory { get; set; }

    // Fumigation
    [Display(Name = "Fumigation Required")]
    public bool RequiresFumigation { get; set; }

    [Display(Name = "Potential Pest Threats")]
    public string? FumigationPestThreats { get; set; }
}
