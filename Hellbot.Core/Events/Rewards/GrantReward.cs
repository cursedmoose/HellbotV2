using Hellbot.Core.Users;

namespace Hellbot.Core.Events.Rewards
{
    public record GrantRewardPayload
    {
        public required UserIdentity Receiver { get; init; }
        public required UserCustomization Reward { get; init; }
    }

    public record GrantReward : HellbotEvent<GrantRewardPayload>;
}
