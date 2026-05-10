using Hellbot.Core.Events;
using Hellbot.Core.Events.Context;
using Hellbot.Core.Users;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.EventBus.Middleware;

public sealed class UserPreferenceEnricher(IEntitlementService entitlements) : IEventMiddleware
{
    public async Task Invoke(IHellbotEvent evt)
    {
        if (evt.Context.User is UserContext uc && uc.Info is User user)
        {
            var snapshot = await entitlements.GetOrLoadPreferencesAsync(user.Id);
            evt.Context = evt.Context with { User = uc with { PreferenceSnapshot = snapshot } };
        }
    }
}
