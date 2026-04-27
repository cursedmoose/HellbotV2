namespace Hellbot.Core.Events.Audio
{
    public record VoiceSegmentCaptured : HellbotEvent<VoiceSegmentPayload>;

    public record VoiceSegmentPayload
    {
        public required DateTimeOffset Start;
        public required DateTimeOffset End;
        public required byte[] RawAudio;
    }
}
