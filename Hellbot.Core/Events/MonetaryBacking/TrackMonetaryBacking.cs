using Hellbot.Core.Events;

namespace Hellbot.Core.Events.MonetaryBacking;

public record TrackMonetaryBackingPayload
{
    public required MonetaryBackingKind Kind { get; init; }
    public required long Amount { get; init; }
    public string? Message { get; init; }
    public required long PointsAwarded { get; init; }
}

public record TrackMonetaryBacking : HellbotEvent<TrackMonetaryBackingPayload>;
