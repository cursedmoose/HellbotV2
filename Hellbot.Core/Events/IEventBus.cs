namespace Hellbot.Core.Events
{
    public interface IEventBus
    {
        Task Publish(IHellbotEvent evt);

        void Subscribe<T>(Func<T, Task> handler) where T : IHellbotEvent;
    }
}
