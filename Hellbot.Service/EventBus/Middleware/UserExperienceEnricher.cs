using Hellbot.Core.Events;
using Hellbot.Core.Users;
using Hellbot.Service.Data.Tables.Users;
using Hellbot.Service.Users;

namespace Hellbot.Service.EventBus.Middleware;

public sealed class UserExperienceEnricher(UserPreferencesTable preferencesTable, UserCache cache) : IEventMiddleware
{
    public async Task Invoke(IHellbotEvent evt)
    {
        if (evt.Context.User is UserContext uc && uc.Info is User user)
        {
            if (!cache.TryGetExperience(user.Id, out var snapshot))
            {
                snapshot = await preferencesTable.ResolveExperienceAsync(user.Id);
                cache.SetExperience(user.Id, snapshot);
            }

            evt.Context = evt.Context with { User = uc with { Experience = snapshot } };
        }
    }
}
