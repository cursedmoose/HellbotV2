using Hellbot.Core.Events;

namespace Hellbot.Service.EventBus.Handlers
{
    public interface IEventHandler
    {
        void Register(IEventBus bus);
    }
}
