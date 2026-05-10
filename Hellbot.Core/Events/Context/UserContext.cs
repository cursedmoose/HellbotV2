using Hellbot.Core.Entitlements;
using Hellbot.Core.Users;

namespace Hellbot.Core.Events.Context;

public record struct UserContext
{
    public User? Info { get; set; }
    public UserPreferenceSnapshot? PreferenceSnapshot { get; set; }
}
