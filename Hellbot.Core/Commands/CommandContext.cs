using Hellbot.Core.Events;

namespace Hellbot.Core.Commands
{
    public record CommandContext
    {
        public required string Command { get; init; }
        public required string[] CommandArgs { get; init; } = [];
        public UserContext? User { get; init; }
        public required EventSource CommandSource { get; init; }
    }
}
