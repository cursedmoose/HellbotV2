using Hellbot.Core.Commands;
using Hellbot.Core.Users;

namespace Hellbot.Service.Commands
{
    public interface ICommandHandler
    {
        public string Command { get; }
        public List<string> Aliases { get; }
        public Role RequiredRole { get; }

        public bool CanHandle(CommandContext context);
        public Task Handle(CommandContext context);
    }
}
