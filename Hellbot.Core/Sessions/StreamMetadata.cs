namespace Hellbot.Core.Sessions
{
    public record StreamMetadata
    {
        public string? Title { get; init; }
        public string? GameName { get; init; }
        public string? Description { get; init; }
    }
}
