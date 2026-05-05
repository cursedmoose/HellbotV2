using Hellbot.Core.Events.Session;
using Hellbot.Core.Sessions;
using Hellbot.Service.Sessions;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class StreamStopHandler(IStreamSessionManager sessionManager, ILogger<StreamStopHandler> logger) : EventHandlerBase<StreamStopped>
    {
        public override Task Handle(StreamStopped evt)
        {
            var channelId = string.IsNullOrEmpty(evt.Data.ChannelId) ? "api" : evt.Data.ChannelId;
            var info = new StreamSessionStopInfo(evt.Source.Platform, channelId);

            if (!sessionManager.RemoveDestination(info, evt.Timestamp, out var ended))
            {
                logger.LogWarning("Stream stop ignored — no destination {Platform}:{Channel}", evt.Source.Platform, channelId);
                return Task.CompletedTask;
            }

            StreamSessionSnapshot? snap = ended != null
                ? StreamSessionSnapshot.From(ended)
                : sessionManager.CurrentStreamSnapshot;
            if (snap != null)
                evt.Context = evt.Context with { Stream = snap };

            if (ended != null)
                logger.LogInformation("Stream Session {SessionId} ended (last destination offline)", ended.Id);
            else
                logger.LogInformation("Stream destination removed {Platform}:{Channel}", evt.Source.Platform, channelId);

            return Task.CompletedTask;
        }
    }
}
