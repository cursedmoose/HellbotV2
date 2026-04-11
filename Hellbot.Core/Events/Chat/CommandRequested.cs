using Hellbot.Core.Commands;

namespace Hellbot.Core.Events.Chat
{        
    public record CommandRequested : HellbotEvent<CommandContext>;
}
