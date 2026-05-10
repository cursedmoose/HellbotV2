using Hellbot.Core.Events;
using Hellbot.Core.Events.Context;
using Hellbot.Core.Users;
using Microsoft.AspNetCore.Http;

namespace Hellbot.Service.Controllers.Filters
{
    public static class UserContextSeeder
    {
        public const string PendingContextItemKey = "Hellbot.UserContextSeed.Pending";

        internal static string? TryParseQuery(IQueryCollection query, out EventContext? context)
        {
            context = null;

            var rawId = query["asHellbotUserId"].ToString().Trim();
            var rawLogin = query["asTwitchLogin"].ToString().Trim();

            Guid? hellbotId = null;
            if (!string.IsNullOrEmpty(rawId))
            {
                if (!Guid.TryParse(rawId, out var g))
                    return "Invalid asHellbotUserId; expected a UUID.";
                hellbotId = g;
            }

            string? twitchLogin = string.IsNullOrWhiteSpace(rawLogin) ? null : rawLogin;

            if (hellbotId.HasValue && twitchLogin is not null)
                return "Specify only one of asHellbotUserId or asTwitchLogin.";

            if (hellbotId.HasValue)
                context = new EventContext
                {
                    Sender = new SenderContext { Locator = new UserLocator.HellbotUser(hellbotId.Value) },
                };
            else if (twitchLogin is not null)
                context = new EventContext
                {
                    Sender = new SenderContext { Locator = new UserLocator.PlatformUsername(PlatformSource.Twitch, twitchLogin) },
                };

            return null;
        }

        public static void ApplyPendingToEvent(HttpContext httpContext, IHellbotEvent evt)
        {
            if (!httpContext.Items.TryGetValue(PendingContextItemKey, out var boxed))
                return;

            httpContext.Items.Remove(PendingContextItemKey);
            if (boxed is EventContext seed)
                evt.Context = seed;
        }
    }
}
