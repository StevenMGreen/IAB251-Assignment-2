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

    // Nature of the package
    public string GoodsType { get; set; } = string.Empty;
    public decimal PackageWidth { get; set; }
    public decimal PackageHeight { get; set; }

    // Nature of the job
    public JobNature JobNature { get; set; }
    public bool RequiresPacking { get; set; }

    // Quarantine details
    public bool RequiresQuarantine { get; set; }
    public string? QuarantineOrigin { get; set; }
    public string? QuarantineTreatmentHistory { get; set; }

    // Fumigation details
    public bool RequiresFumigation { get; set; }
    public string? FumigationPestThreats { get; set; }

    public string Status { get; set; } = "Pending";
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
