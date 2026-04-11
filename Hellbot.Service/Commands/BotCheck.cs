using Hellbot.Core.Commands;

namespace Hellbot.Service.Commands
{
    public class BotCheck(ILogger<BotCheck> logger) : CommandHandler(logger)
    {
        public override List<string> Aliases => ["hello"];

        public override string Command => "botcheck";

        public override Role RequiredRole => Role.None;
    }
}
