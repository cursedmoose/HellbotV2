using Hellbot.Core.Events;
using Hellbot.Core.Users;
using Hellbot.Service.Entitlements;

namespace Hellbot.Service.EventBus.Middleware;

public sealed class UserExperienceEnricher(IEntitlementService entitlements) : IEventMiddleware
{
    public async Task Invoke(IHellbotEvent evt)
    {
        if (evt.Context.User is UserContext uc && uc.Info is User user)
        {
            var snapshot = await entitlements.GetOrLoadExperienceSnapshotAsync(user.Id);
            evt.Context = evt.Context with { User = uc with { Experience = snapshot } };
        }
    }
}
