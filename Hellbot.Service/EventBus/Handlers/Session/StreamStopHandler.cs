using Hellbot.Core.Events.Session;
using Hellbot.Core.Sessions;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class StreamStopHandler(IStreamSessionManager sessionManager, ILogger<StreamStopHandler> logger) : EventHandlerBase<StreamStopped>
    {
        public override Task Handle(StreamStopped evt)
        {
            var stream = sessionManager.EndSession(evt.Timestamp);
            logger.LogInformation("Stream Session {SessionId} Stopped", stream?.Id);
            return Task.CompletedTask;
        }
    }
}
