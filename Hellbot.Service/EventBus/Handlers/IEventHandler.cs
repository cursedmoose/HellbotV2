using Hellbot.Core.Events;

namespace Hellbot.Service.EventBus.Handlers
{
    public interface IEventHandler
    {
        bool CanHandle(IHellbotEvent evt);
        Task Handle(IHellbotEvent evt);
    }
}
