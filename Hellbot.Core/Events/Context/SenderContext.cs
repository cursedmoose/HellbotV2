using Hellbot.Core.Users;

namespace Hellbot.Core.Events.Context;

/// <summary>Optional overlay describing who originated an event (identity snapshot and/or resolution locator).</summary>
public readonly record struct SenderContext
{
    public UserIdentity? Identity { get; init; }
    public UserLocator? Locator { get; init; }
}
