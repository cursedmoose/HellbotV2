namespace Hellbot.Core.Events.Users
{
    public record UserSubscribedPayload
    {
        public required string Tier { get; init; }
    }

    public record UserSubscribed : HellbotEvent<UserSubscribedPayload>;
}
