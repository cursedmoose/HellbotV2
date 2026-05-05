namespace Hellbot.Core.Sessions
{
    public record StreamSessionSnapshot(
        Guid Id,
        DateTimeOffset StartedAt,
        DateTimeOffset? EndedAt,
        StreamMetadata Metadata,
        IReadOnlyList<StreamDestination> Destinations)
    {
        public static StreamSessionSnapshot From(StreamSession session) => new(
            session.Id,
            session.StartedAt,
            session.EndedAt,
            session.Metadata,
            session.Destinations.ToList());
    }
}
