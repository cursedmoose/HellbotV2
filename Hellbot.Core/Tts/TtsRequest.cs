namespace Hellbot.Core.Tts
{
    public record TtsRequest
    {
        public required Guid RequestId { get; init; }
        public required string Message { get; init; }
        public required string VoiceKey { get; init; }
        public VoiceSettings? VoiceSettings { get; init; }
        public string? SceneId { get; init; }
    }
}
