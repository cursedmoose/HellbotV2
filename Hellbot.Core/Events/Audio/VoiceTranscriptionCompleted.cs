namespace Hellbot.Core.Events.Audio
{
    public record VoiceTranscriptionPayload
    {
        public required string Text { get; init; }
        public required DateTimeOffset Start { get; init; }
        public required DateTimeOffset End { get; init; }
    }
    public record VoiceTranscriptionCompleted : HellbotEvent<VoiceTranscriptionPayload>;
}
