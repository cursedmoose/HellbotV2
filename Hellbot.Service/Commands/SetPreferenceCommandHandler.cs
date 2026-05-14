using Hellbot.Core.Commands;
using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Core.Events.Preferences;
using Hellbot.Core.Users;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.Commands;

public sealed class SetPreferenceCommandHandler(
    IEventBus bus,
    IEntitlementService entitlements,
    ILogger<SetPreferenceCommandHandler> logger) : CommandHandler(logger)
{
    public override List<string> Aliases => [];

    public override string Command => "set";

    public override Role RequiredRole => Role.None;

    public override async Task Handle(CommandContext context)
    {
        if (context.CommandArgs.Length < 1)
        {
            await SendAsync(context, "Usage: !set voice <name>");
            return;
        }

        if (context.CommandArgs[0].Equals("voice", CompareBy))
        {
            await HandleSetVoiceAsync(context);
            return;
        }

        await SendAsync(context, "Unknown option. Try: !set voice <name>");
    }

    private async Task HandleSetVoiceAsync(CommandContext context)
    {
        if (context.User?.Info is not User user)
        {
            await SendAsync(context, "Could not resolve your account; try again later.");
            return;
        }

        if (context.CommandArgs.Length < 2)
        {
            await SendAsync(context, "Usage: !set voice <name>");
            return;
        }

        var slug = context.CommandArgs[1];
        if (string.IsNullOrWhiteSpace(slug))
        {
            await SendAsync(context, "Usage: !set voice <name>");
            return;
        }

        var entitlementId = NormalizeVoiceEntitlementId(slug);
        var snap = await entitlements.GetCapabilitiesAsync(user.Id);
        var match = snap.Entitlements.FirstOrDefault(e =>
            e.CatalogItem.EntitlementType == EntitlementType.TtsVoice
            && e.CatalogItem.IsActive
            && string.Equals(e.CatalogItem.EntitlementId, entitlementId, StringComparison.OrdinalIgnoreCase));

        if (match is null)
        {
            await SendAsync(
                context,
                $"You do not have access to voice '{entitlementId}'. Use !get voice to see your voices.");
            return;
        }

        await bus.Publish(new SetUserPreference
        {
            Source = context.CommandSource,
            Context = new EventContext { User = context.User },
            Data = new SetUserPreferencePayload
            {
                EntitlementType = EntitlementType.TtsVoice,
                SelectedEntitlementCatalogId = match.CatalogItem.Id,
            },
        });

        logger.LogInformation(
            "User {UserId} set TTS voice to {VoiceId} via chat.",
            user.Id,
            match.CatalogItem.EntitlementId);
        await SendAsync(context, $"TTS voice set to {match.CatalogItem.EntitlementId}.");
    }

    private static string NormalizeVoiceEntitlementId(string slug)
    {
        return slug.Contains('/') ? slug : $"voice/{slug}";
    }

    private Task SendAsync(CommandContext context, string message)
    {
        return bus.Publish(new SendChatMessage
        {
            Source = context.CommandSource,
            Data = new SendChatPayload
            {
                Channel = context.CommandSource,
                Message = message,
            },
        });
    }
}
