namespace Hellbot.Core.Sessions
{
    public class StreamSession
    {
        public Guid Id { get; set; }
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }

        public bool IsActive => EndedAt == null;
    }
}
