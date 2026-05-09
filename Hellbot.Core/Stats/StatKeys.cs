namespace Hellbot.Core.Stats;

/// <summary>
/// Canonical stat keys persisted in <c>user_stats.stat_key</c>. P0 prototype only.
/// </summary>
public static class StatKeys
{
    /// <seealso cref="Hellbot.Core.Events.Chat.ChatMessageReceived"/>
    public const string ChatMessagesSent = "chat_messages_sent";

    /// <seealso cref="Hellbot.Core.Events.Chat.CommandRequested"/>
    public const string CommandsUsed = "commands_used";

    /// <seealso cref="Hellbot.Core.Events.Users.UserSubscribed"/>
    public const string TimesSubscribed = "times_subscribed";
}
