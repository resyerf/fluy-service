namespace Fluy.Infrastructure.External.Services;

public class SubscriptionRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid PlanId { get; set; }
    public string Status { get; set; } = null!;
}
