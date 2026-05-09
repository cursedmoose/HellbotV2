using Hellbot.Core.Events.Chat;
using Hellbot.Core.Stats;
using Hellbot.Service.Stats;

namespace Hellbot.Service.EventBus.Handlers.Stats;

public sealed class CommandRequestedStatsHandler(IUserStatsRecorder stats)
    : EventHandlerBase<CommandRequested>
{
    public override Task Handle(CommandRequested evt)
    {
        if (!evt.Context.TryGetPersistedUser(out var user))
            return Task.CompletedTask;

        stats.Increment(user.Id, StatKeys.CommandsUsed, streamSessionId: evt.Context.StreamSessionId);
        return Task.CompletedTask;
    }
}
