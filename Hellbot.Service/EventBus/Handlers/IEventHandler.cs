namespace Hellbot.Service.EventBus.Handlers
{
    public interface IEventHandler
    {
        Task Handle(IEventHandler evt);
    }
}
