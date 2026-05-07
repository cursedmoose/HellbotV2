using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;
using Hellbot.Service.Sessions;
namespace Hellbot.Service.EventBus.Middleware
{
    public class StreamSessionContextEnricher(IStreamSessionManager sessionManager) : IEventMiddleware
    {
        public Task Invoke(IHellbotEvent evt)
        {
            if (evt is StreamStarted)
                return Task.CompletedTask;

            var snapshot = sessionManager.CurrentStreamSnapshot;
            if (snapshot != null)
                evt.Context = evt.Context with { Stream = snapshot };

            return Task.CompletedTask;
        }
    }
}
