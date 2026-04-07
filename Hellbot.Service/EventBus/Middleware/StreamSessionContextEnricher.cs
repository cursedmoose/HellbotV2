using Hellbot.Core.Events;
using Hellbot.Core.Sessions;

namespace Hellbot.Service.EventBus.Middleware
{
    public class StreamSessionContextEnricher(IStreamSessionManager sessionManager): IEventMiddleware
    {
        public async Task Invoke(IHellbotEvent evt)
        {
            evt.StreamId = sessionManager.CurrentSessionId;
        }
    }
}
