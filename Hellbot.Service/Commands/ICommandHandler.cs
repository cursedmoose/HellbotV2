using Hellbot.Core.Commands;

namespace Hellbot.Service.Commands
{
    public interface ICommandHandler
    {
        public string Command { get; }
        public List<string> Aliases { get; }
        public Role RequiredRole { get; }

        public bool CanHandle(CommandContext context);
        public void Handle(CommandContext context);
    }
}
