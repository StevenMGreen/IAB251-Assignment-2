using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InterportCargo.Web.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.IsInRole("QuotationOfficer"))
            return RedirectToPage("/Employee/Quotations");

        if (User.IsInRole("Customer"))
            return RedirectToPage("/Customers/Dashboard");

        return Page();
    }
}
