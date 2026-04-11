using Hellbot.Core.Commands;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;

namespace Hellbot.Service.Commands
{
    public class BotCheck(IEventBus bus, ILogger<BotCheck> logger) : CommandHandler(logger)
    {
        public override List<string> Aliases => ["hello"];

        public override string Command => "botcheck";

        public override Role RequiredRole => Role.None;

        public override void Handle(CommandContext context)
        {
            bus.Publish(new SendChatMessage
            {
                Data = new()
                {
                    Channel = context.CommandSource,
                    Message = "Hello from Bot!",
                },
                Source = context.CommandSource
            });
        }
    }
}
