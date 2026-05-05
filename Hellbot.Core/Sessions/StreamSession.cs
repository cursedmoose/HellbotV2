namespace Hellbot.Core.Sessions
{
    public class StreamSession
    {
        public Guid Id { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }
        public StreamMetadata Metadata { get; set; } = new();
        public List<StreamDestination> Destinations { get; } = new();

        public bool IsActive => EndedAt == null;
    }
}
