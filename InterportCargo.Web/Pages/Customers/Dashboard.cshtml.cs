using System.Security.Claims;
using InterportCargo.Web.Data;
using InterportCargo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace InterportCargo.Web.Pages.Customers;

[Authorize(Roles = "Customer")]
public class DashboardModel(AppDbContext db) : PageModel
{
    public string CustomerName { get; set; } = string.Empty;
    public List<Message> UnreadMessages { get; set; } = [];
    public List<QuotationRequest> QuotationRequests { get; set; } = [];
    public bool JustSubmitted { get; set; }

    public async Task OnGetAsync(bool submitted = false)
    {
        JustSubmitted = submitted;
        CustomerName = User.Identity?.Name ?? string.Empty;

        var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        UnreadMessages = await db.Messages
            .Where(m => m.CustomerId == customerId && !m.IsRead)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        foreach (var msg in UnreadMessages)
            msg.IsRead = true;

        QuotationRequests = await db.QuotationRequests
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();

        await db.SaveChangesAsync();
    }
}
