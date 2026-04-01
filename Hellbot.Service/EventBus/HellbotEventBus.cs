using Hellbot.Core.Events;
using Hellbot.Service.EventBus.Handlers.Global;

namespace Hellbot.Service.EventBus
{
    public class HellbotEventBus : IEventBus
    {
        private readonly ILogger<EventLogger> _logger;

        public HellbotEventBus(ILogger<EventLogger> logger)
        {
            _logger = logger;
            _logger.LogInformation($"EventBus created: {GetHashCode()}");
        }
        private readonly Dictionary<Type, List<Func<IHellbotEvent, Task>>> _handlers = new();
        private readonly List<Func<IHellbotEvent, Task>> _globalHandlers = new();

        public async Task Publish(IHellbotEvent evt)
        {
            var eventType = evt.GetType();

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
        }

        public void SubscribeAll(Func<IHellbotEvent, Task> handler)
        {
            _globalHandlers.Add(handler);
        }
    }
}
