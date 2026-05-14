namespace Hellbot.Core.Events.Rewards;

public sealed record StartRumorRewardRedeemed : TwitchRewardRedeemed;

public sealed record LoseSweetrollsRewardRedeemed : TwitchRewardRedeemed;

public sealed record RequestTtsVoiceRewardRedeemed : TwitchRewardRedeemed;

public sealed record StopStreamRewardRedeemed : TwitchRewardRedeemed;
