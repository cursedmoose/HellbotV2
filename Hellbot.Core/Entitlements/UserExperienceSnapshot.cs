using System.Collections.ObjectModel;

namespace Hellbot.Core.Entitlements;

/// <summary>
/// Resolved entitlement selections usable at event handling time (e.g. equipped TTS voice/scene catalog rows).
/// </summary>
public sealed record UserExperienceSnapshot
{
    private static readonly ReadOnlyDictionary<EntitlementType, EntitlementCatalogItem> EmptySelections = new(new Dictionary<EntitlementType, EntitlementCatalogItem>());

    public static UserExperienceSnapshot Empty { get; } = new() { Selections = EmptySelections };

    /// <summary>At most one entry per <see cref="EntitlementType"/>.</summary>
    public required IReadOnlyDictionary<EntitlementType, EntitlementCatalogItem> Selections { get; init; }

    public EntitlementCatalogItem? GetOrDefault(EntitlementType type)
        => Selections.TryGetValue(type, out var item) ? item : null;
}
