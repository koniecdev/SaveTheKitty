namespace SaveTheKitty.API.Entities.Common;
public class AuditableEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? Modified { get; set; }
    public string? ModifiedBy { get; set; }
    public int StatusId { get; set; } = 1;
    public DateTimeOffset? Inactivated { get; set; }
    public string? InactivatedBy { get; set; }
}
