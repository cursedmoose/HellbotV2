namespace Hellbot.Core.Events
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T evt) where T : IHellbotEvent;
        void Subscribe<T>(Func<T, Task> handler) where T : IHellbotEvent;
        void SubscribeAll(Func<IHellbotEvent, Task> handler);
    }
}
