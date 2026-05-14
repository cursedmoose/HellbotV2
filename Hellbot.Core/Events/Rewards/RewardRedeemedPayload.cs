namespace Hellbot.Core.Events.Rewards;

public record RewardRedeemedPayload
{
    public required long Cost { get; init; }
    public required string CorrelationId { get; init; }
    public required string RewardId { get; init; }
    public required string RedemptionId { get; init; }
    public string? RewardTitle { get; init; }
    public string? UserInput { get; init; }
}
