using Hellbot.Core.Commands;
using Hellbot.Core.Entitlements;
using Hellbot.Core.Events;
using Hellbot.Core.Events.Chat;
using Hellbot.Core.Users;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.Commands;

public sealed class GetPreferencesCommandHandler(
    IEventBus bus,
    IEntitlementService entitlements,
    ILogger<GetPreferencesCommandHandler> logger) : CommandHandler(logger)
{
    public override List<string> Aliases => [];

    public override string Command => "get";

    public override Role RequiredRole => Role.None;

    public override async Task Handle(CommandContext context)
    {
        if (context.CommandArgs.Length < 1)
        {
            await SendAsync(context, "Usage: !get voice | !get avatar");
            return;
        }

        if (!TryResolveKindArg(context.CommandArgs[0], out var kind))
        {
            await SendAsync(context, "Unknown option. Try: !get voice, !get avatar");
            return;
        }

        if (context.User?.Info is not User user)
        {
            await SendAsync(context, "Could not resolve your account; try again later.");
            return;
        }

        var snap = await entitlements.GetCapabilitiesAsync(user.Id);
        var equipped = snap.PreferenceSnapshot.GetOrDefault(kind);
        var current = equipped?.EntitlementId ?? "none";
        var availableIds = snap.Entitlements
            .Where(e => e.CatalogItem.EntitlementType == kind && e.CatalogItem.IsActive)
            .Select(e => e.CatalogItem)
            .GroupBy(c => c.Id)
            .Select(g => g.First())
            .OrderBy(c => c.EntitlementId, StringComparer.OrdinalIgnoreCase)
            .Select(c => c.EntitlementId)
            .ToList();

        var availableList = availableIds.Count == 0 ? "none" : string.Join(", ", availableIds);
        var availableTitle = kind switch
        {
            EntitlementType.TtsVoice => "voices",
            EntitlementType.TtsAvatar => "avatars",
            _ => "items",
        };

        logger.LogInformation(
            "User {UserId} listed preferences for {Kind} via chat ({Count} available).",
            user.Id,
            kind,
            availableIds.Count);

        await SendAsync(
            context,
            $"current: {current} | available {availableTitle}: {availableList}");
    }

    private static bool TryResolveKindArg(string arg, out EntitlementType kind)
    {
        if (arg.Equals("voice", CompareBy) || arg.Equals("voices", CompareBy))
        {
            kind = EntitlementType.TtsVoice;
            return true;
        }

        if (arg.Equals("avatar", CompareBy) || arg.Equals("avatars", CompareBy))
        {
            kind = EntitlementType.TtsAvatar;
            return true;
        }

        kind = default;
        return false;
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
