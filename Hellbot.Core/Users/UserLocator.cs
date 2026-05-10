using System.Text.Json.Serialization;
using Hellbot.Core.Events;

namespace Hellbot.Core.Users;

/// <summary>Ways to address a Hellbot user for resolution (internal id, immutable platform account id, or mutable username).</summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$kind")]
[JsonDerivedType(typeof(HellbotUser), nameof(HellbotUser))]
[JsonDerivedType(typeof(PlatformAccount), nameof(PlatformAccount))]
[JsonDerivedType(typeof(PlatformUsername), nameof(PlatformUsername))]
public abstract record UserLocator
{
    /// <summary>Internal <c>users.id</c> (Hellbot user id).</summary>
    public sealed record HellbotUser(Guid Id) : UserLocator;

    /// <summary>Stable platform-specific account identifier (stored as <c>platform_user_id</c>), not Hellbot user id.</summary>
    public sealed record PlatformAccount(PlatformSource Platform, string PlatformAccountId) : UserLocator;

    /// <summary>Resolve-only: match <c>platform_user_name</c>. Must match exactly one row or resolution fails.</summary>
    public sealed record PlatformUsername(PlatformSource Platform, string Username) : UserLocator;

    /// <summary>Resolution lookup uses platform + immutable platform account id; <see cref="UserIdentity.Username"/> is ignored.</summary>
    public static UserLocator FromIdentity(UserIdentity identity) =>
        new PlatformAccount(identity.Platform, identity.UserId);
}
