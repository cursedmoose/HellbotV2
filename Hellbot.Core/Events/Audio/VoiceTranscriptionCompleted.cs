namespace Hellbot.Core.Events.Audio
{
    public record VoiceTranscriptionPayload
    {
        public required string Text;
        public required DateTimeOffset Start;
        public required DateTimeOffset End;
    }
    public record VoiceTranscriptionCompleted : HellbotEvent<VoiceTranscriptionPayload>;
}
