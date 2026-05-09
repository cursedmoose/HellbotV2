namespace Hellbot.Service.Users.Identity;

/// <summary>Outcome of resolving a <see cref="UserLocator"/> to a Hellbot user id.</summary>
public abstract record UserResolutionResult
{
    public sealed record Resolved(Guid HellbotUserId) : UserResolutionResult;

    public sealed record NotFound : UserResolutionResult;

    /// <summary>More than one <c>user_identities</c> row matched the platform username.</summary>
    public sealed record AmbiguousUsername(IReadOnlyList<Guid> CandidateHellbotUserIds) : UserResolutionResult;
}
