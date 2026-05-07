using Hellbot.Core.Events.Session;
using Hellbot.Core.Sessions;
using Hellbot.Service.Sessions;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class StreamStartHandler(IStreamSessionManager sessionManager, ILogger<StreamStartHandler> logger) : EventHandlerBase<StreamStarted>
    {
        public override Task Handle(StreamStarted evt)
        {
            var channelId = string.IsNullOrEmpty(evt.Data.ChannelId) ? "api" : evt.Data.ChannelId;
            var info = new StreamSessionStartInfo(
                evt.Timestamp,
                evt.Source.Platform,
                new StreamMetadata
                {
                    Title = evt.Data.Title,
                    GameName = evt.Data.GameName,
                    Description = evt.Data.Description
                },
                new StreamDestination(
                    evt.Source.Platform,
                    channelId,
                    evt.Timestamp,
                    ExternalBroadcastId: evt.Data.ExternalBroadcastId,
                    Url: evt.Data.DestinationUrl));

            sessionManager.StartOrAddDestination(info);
            evt.Context = evt.Context with { Stream = sessionManager.CurrentStreamSnapshot };

            logger.LogInformation(
                "Stream Session {SessionId} — destination {Platform}:{Channel}",
                sessionManager.CurrentSessionId,
                evt.Source.Platform,
                channelId);
            return Task.CompletedTask;
        }
    }
}
