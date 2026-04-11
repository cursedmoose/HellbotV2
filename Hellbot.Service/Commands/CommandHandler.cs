using Hellbot.Core.Commands;

namespace Hellbot.Service.Commands
{
    public abstract class CommandHandler(ILogger<CommandHandler> logger): ICommandHandler
    {
        public abstract List<string> Aliases { get; }
        public abstract string Command { get; }
        public abstract Role RequiredRole { get; }

        protected const StringComparison CompareBy = StringComparison.OrdinalIgnoreCase;

        public bool CanHandle(CommandContext context)
        {
            return (MatchesCommand(context) || MatchesAlias(context))
                && UserHasPermissions(context)
                && MeetsCommandRequirements(context);
        }

        public virtual void Handle(CommandContext context)
        {
            logger.LogInformation("Handling {Command}! Coming soon!!", Command);
        }


        public virtual bool MeetsCommandRequirements(CommandContext context) { return true; }

        private bool MatchesCommand(CommandContext context)
        {
            return Command.Equals(context.Command, CompareBy);
        }

        private bool MatchesAlias(CommandContext context)
        {
            return Aliases.Any((command) => command.Equals(context.Command, CompareBy));
        }

        private bool UserHasPermissions(CommandContext context)
        {
            return context.UserRole >= RequiredRole;
        }
    }
}
