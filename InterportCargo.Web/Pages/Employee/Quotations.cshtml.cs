using InterportCargo.Web.Data;
using InterportCargo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InterportCargo.Web.Pages.Employee;

[Authorize(Roles = "QuotationOfficer")]
public class QuotationsModel(AppDbContext db) : PageModel
{
    public List<QuotationRequest> Requests { get; set; } = [];

    public async Task OnGetAsync()
    {
        Requests = await db.QuotationRequests
            .Include(r => r.Customer)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }
}
