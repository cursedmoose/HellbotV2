namespace Hellbot.Core.Events
{
    public interface IEventBus
    {
        Task Publish(IHellbotEvent evt);
    }
}
