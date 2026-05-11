using Hellbot.Core.Events.Context;
using Hellbot.Core.Events.MonetaryBacking;
using Hellbot.Core.Stats;
using Hellbot.Service.EventBus.Handlers;
using Hellbot.Service.Stats;

namespace Hellbot.Service.EventBus.Handlers.Stats;

public sealed class TrackMonetaryBackingStatsHandler(IUserStatsRecorder stats)
    : EventHandlerBase<TrackMonetaryBacking>
{
    public override Task Handle(TrackMonetaryBacking evt)
    {
        if (evt.Data.PointsAwarded == 0)
            return Task.CompletedTask;

        if (!evt.Context.TryGetUser(out var user))
            return Task.CompletedTask;

        stats.Increment(user.Id, StatKeys.BackerPoints, delta: evt.Data.PointsAwarded, streamSessionId: evt.Context.Stream?.Id);
        return Task.CompletedTask;
    }
}
