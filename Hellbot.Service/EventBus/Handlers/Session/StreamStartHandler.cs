using Hellbot.Core.Events.Session;
using Hellbot.Core.Sessions;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class StreamStartHandler(IStreamSessionManager sessionManager, ILogger<StreamStartHandler> logger) : EventHandlerBase<StreamStarted>
    {
        public override Task Handle(StreamStarted evt)
        {
            var stream = sessionManager.StartSession(evt.Timestamp);
            logger.LogInformation("Stream Session {SessionId} Started", stream.Id);
            return Task.CompletedTask;
        }
    }
}
