using Hellbot.Core.Events;
using Hellbot.Core.Sessions;

namespace Hellbot.Service.EventBus.Enrichers
{
    public class SessionContextEnricher(IStreamSessionManager sessionManager)
    {
        public void Enrich(IHellbotEvent evt)
        {
            evt.StreamId = sessionManager.CurrentSessionId;
        }
    }
}
