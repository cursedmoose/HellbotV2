using Hellbot.Core.Events;

namespace Hellbot.Core.Events.Rewards;

public abstract record RewardRedeemed : HellbotEvent<RewardRedeemedPayload>;
