using Hellbot.Core.Events;
using Hellbot.Service.EventBus.Handlers.Global;

namespace Hellbot.Service.EventBus
{
    public class HellbotEventBus(IEnumerable<IEventMiddleware> middlewares, ILogger<HellbotEventBus> logger) : IEventBus
    {
        private readonly Dictionary<Type, List<Func<IHellbotEvent, Task>>> _handlers = [];
        public async Task Publish(IHellbotEvent evt)
        {
            var eventType = evt.GetType();

            foreach (var middleware in middlewares)
            {
                await middleware.Invoke(evt);
            }

            foreach (var (registeredType, handlers) in _handlers)
            {
                if (registeredType.IsAssignableFrom(eventType))
                {
                    foreach (var handler in handlers)
                    {
                        await handler(evt);
                    }
                }
            }
        }

        public void Subscribe<T>(Func<T, Task> handler) where T : IHellbotEvent
        {
            var type = typeof(T);

            if (!_handlers.ContainsKey(type))
                _handlers[type] = [];

            _handlers[type].Add(evt => handler((T)evt));
            logger.LogInformation("Subscribed {Handler} to {Event}", handler.Method.DeclaringType?.Name, type.Name);
        }
    }
}
