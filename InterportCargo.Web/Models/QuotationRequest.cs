namespace InterportCargo.Web.Models;

public enum JobNature
{
    Import,
    Export
}

public class QuotationRequest
{
    public int Id { get; set; }
    public string RequestReference { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public int NumberOfContainers { get; set; }
    public string PackageNature { get; set; } = string.Empty;

    public JobNature JobNature { get; set; }
    public bool RequiresPacking { get; set; }
    public bool RequiresQuarantine { get; set; }

    public string Status { get; set; } = "Pending";
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
