namespace InterportCargo.Web.Models;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<QuotationRequest> QuotationRequests { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
}
