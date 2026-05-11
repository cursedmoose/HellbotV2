using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Hellbot.UI.Services;

/// <summary>
/// Starts the backend events hub for the Blazor circuit as soon as it opens, so buffered projections
/// stay live while the user is on any interactive route (not only Chat/Events/Service status).
/// </summary>
public sealed class EventFeedCircuitHandler : CircuitHandler
{
    private readonly EventFeed _feed;

    public EventFeedCircuitHandler(EventFeed feed) => _feed = feed;

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        => _feed.EnsureStartedAsync();
}
