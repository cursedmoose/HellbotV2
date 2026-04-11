using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;
using Hellbot.Core.Sessions;

namespace Hellbot.Service.EventBus.Handlers.Session
{
    public class StreamStartHandler(IStreamSessionManager sessionManager, ILogger<StreamStartHandler> logger) : IEventHandler
    {
        public void Register(IEventBus bus)
        {
            bus.Subscribe<StreamStarted>(Handle);
        }

        private Task Handle(StreamStarted evt)
        {
            var stream = sessionManager.StartSession(evt.Timestamp);
            logger.LogInformation("Stream Session {SessionId} Started", stream.Id);
            return Task.CompletedTask;
        }
    }
}
