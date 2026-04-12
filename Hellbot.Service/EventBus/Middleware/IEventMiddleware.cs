namespace Hellbot.Core.Events
{
    public interface IEventMiddleware
    {
        Task Invoke(IHellbotEvent evt);
    }
}
