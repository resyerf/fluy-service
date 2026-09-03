namespace Fluy.Infrastructure.External.Services;

public class PlanFeatureRow
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public Guid FeatureId { get; set; }
    public string Value { get; set; } = null!;
}
