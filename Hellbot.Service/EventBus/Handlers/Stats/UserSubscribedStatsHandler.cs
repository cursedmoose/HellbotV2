using Hellbot.Core.Events.Users;
using Hellbot.Core.Stats;
using Hellbot.Service.Stats;

namespace Hellbot.Service.EventBus.Handlers.Stats;

public sealed class UserSubscribedStatsHandler(IUserStatsRecorder stats) : EventHandlerBase<UserSubscribed>
{
    public override Task Handle(UserSubscribed evt)
    {
        if (!evt.Context.TryGetPersistedUser(out var user))
            return Task.CompletedTask;

        stats.Increment(user.Id, StatKeys.TimesSubscribed, streamSessionId: evt.Context.StreamSessionId);
        return Task.CompletedTask;
    }
}
