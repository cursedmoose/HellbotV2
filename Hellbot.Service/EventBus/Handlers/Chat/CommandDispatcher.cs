using Hellbot.Core.Events.Chat;
using Hellbot.Service.Commands;

namespace Hellbot.Service.EventBus.Handlers.Chat
{
    public class CommandDispatcher(IEnumerable<ICommandHandler> handlers, ILogger<CommandDispatcher> logger) : EventHandlerBase<CommandRequested>
    {
        private readonly Dictionary<string, ICommandHandler> _handlerMap = handlers
                .SelectMany(h => GetKeys(h)
                    .Select(key => (key, handler: h)))
                .ToDictionary(
                    x => x.key,
                    x => x.handler,
                    StringComparer.OrdinalIgnoreCase);

        private static IEnumerable<string> GetKeys(ICommandHandler handler)
        {
            yield return handler.Command;

            if (handler.Aliases != null)
            {
                foreach (var alias in handler.Aliases)
                    yield return alias;
            }
        }
        public override async Task Handle(CommandRequested evt)
        {
            if (!_handlerMap.TryGetValue(evt.Data.Command, out var handler))
            {
                logger.LogInformation("No handler found for {Command}.", evt.Data.Command);
                return;
            }

            var data = evt.Data with { Command = handler.Command };
            if (handler.CanHandle(data))
            {
                handler.Handle(data);
                return;
            }
        }
    }
}
