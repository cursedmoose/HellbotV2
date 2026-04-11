using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;
using Hellbot.Core.Sessions;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class StreamStopHandler(IStreamSessionManager sessionManager, ILogger<StreamStopHandler> logger) : IEventHandler
    {
        public void Register(IEventBus bus)
        {
            bus.Subscribe<StreamStopped>(Handle);
        }

        private Task Handle(StreamStopped evt)
        {
            var stream = sessionManager.EndSession(evt.Timestamp);
            logger.LogInformation("Stream Session {SessionId} Stopped", stream?.Id);
            return Task.CompletedTask;
        }
    }
}
