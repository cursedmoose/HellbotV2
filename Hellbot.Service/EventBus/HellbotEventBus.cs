using Hellbot.Core.Events;
using Hellbot.Service.EventBus.Handlers;

namespace Hellbot.Service.EventBus
{
    public class HellbotEventBus(IServiceScopeFactory serviceScope, ILogger<HellbotEventBus> logger) : IEventBus
    {
        private readonly Dictionary<Type, List<Func<IHellbotEvent, Task>>> _handlers = [];
        public async Task Publish(IHellbotEvent evt)
        {
            using var scope = serviceScope.CreateScope();
            var middlewares = scope.ServiceProvider.GetServices<IEventMiddleware>();
            var eventType = evt.GetType();
            var handlers = scope.ServiceProvider.GetServices<IEventHandler>().Where(h => h.CanHandle(evt));

            foreach (var middleware in middlewares)
            {
                await middleware.Invoke(evt);
            }

            foreach (var handler in handlers)
            {
                try
                {
                    await handler.Handle(evt);
                }
                catch(Exception e)
                {
                    logger.LogError("{Handler} failed: {Exception}", handler.GetType(), e.Message);
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
