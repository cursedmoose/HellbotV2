namespace Hellbot.Core.Events.Users
{
    public record UserSubscribedPayload
    {
        public required string SubscriberUserId { get; init; }
        public required string SubscriberUserName { get; init; }
        public required string Tier { get; init; }
    }

    public record UserSubscribed : HellbotEvent<UserSubscribedPayload>;
}
