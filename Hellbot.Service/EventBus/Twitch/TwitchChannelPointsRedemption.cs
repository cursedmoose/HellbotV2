namespace Hellbot.Service.EventBus.Twitch;

/// <summary>
/// Neutral input for <see cref="TwitchChannelPointsRedemptionInterpreter"/> (map from TwitchLib EventSub when wired).
/// </summary>
public sealed record TwitchChannelPointsRedemption(
    string RewardId,
    string Title,
    long Cost,
    string? UserInput,
    string RedemptionId);
