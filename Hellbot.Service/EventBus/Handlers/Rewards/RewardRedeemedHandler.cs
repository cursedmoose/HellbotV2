using Hellbot.Core.Events.Rewards;
using Hellbot.Service.EventBus.Handlers;

namespace Hellbot.Service.EventBus.Handlers.Rewards;

public sealed class RewardRedeemedHandler(ILogger<RewardRedeemedHandler> logger) : EventHandlerBase<RewardRedeemed>
{
    public override Task Handle(RewardRedeemed evt)
    {
        logger.LogInformation(
            "Reward redeemed {ConcreteType} eventId={EventId} source={Source} correlationId={CorrelationId} rewardId={RewardId} cost={Cost}",
            evt.GetType().Name,
            evt.Id,
            evt.Source,
            evt.Data.CorrelationId,
            evt.Data.RewardId,
            evt.Data.Cost);
        return Task.CompletedTask;
    }
}
