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
        public async Task PublishAsync<T>(T evt) where T : IHellbotEvent
        {
            var type = typeof(T);

            var tasks = new List<Task>();

            if (_handlers.TryGetValue(type, out var handlers))
            {
                tasks.AddRange(handlers.Select(h => h(evt)));
            }

            // Global handlers
            tasks.AddRange(_globalHandlers.Select(h => h(evt)));

            await Task.WhenAll(tasks);
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
