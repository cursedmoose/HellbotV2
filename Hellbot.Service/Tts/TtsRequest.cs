using Hellbot.Core.TTS;

namespace Hellbot.Service.Tts
{
    public record TtsRequest
    {
        public required Guid RequestId { get; init; }
        public required string Message { get; init; }
        public required string VoiceId { get; init; }
        public VoiceSettings VoiceSettings { get; init; } = new();
        public string? SceneId { get; init; }
    }
}
