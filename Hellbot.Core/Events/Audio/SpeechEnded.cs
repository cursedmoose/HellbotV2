namespace Hellbot.Core.Events.Audio
{
    public record SpeechEnded : HellbotEvent<SpeechEndedPayload>;

    public record SpeechEndedPayload
    {
        public required TimeSpan Duration;
    }

}
