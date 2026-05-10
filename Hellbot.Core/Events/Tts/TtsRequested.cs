namespace Hellbot.Core.Events.Tts
{
    public record TtsRequestPayload
    {
        public required string Text { get; init; }
        public int Priority { get; init; } = 0;
    }

    public record TtsRequested : HellbotEvent<TtsRequestPayload>;
}
