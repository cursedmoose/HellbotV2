using Hellbot.Core.Events.Chat;
using Hellbot.Core.Stats;
using Hellbot.Service.Stats;

namespace Hellbot.Service.EventBus.Handlers.Stats;

public sealed class ChatMessageReceivedStatsHandler(IUserStatsRecorder stats) : EventHandlerBase<ChatMessageReceived>
{
    public override Task Handle(ChatMessageReceived evt)
    {
        if (!evt.Context.TryGetUser(out var user))
            return Task.CompletedTask;

        stats.Increment(user.Id, StatKeys.ChatMessagesSent, streamSessionId: evt.Context.Stream?.Id);
        return Task.CompletedTask;
    }
}
