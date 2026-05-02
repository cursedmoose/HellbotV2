using System.Text.Json.Serialization;

namespace Hellbot.Core.Events.Audio
{
    public record VoiceSegmentCaptured : HellbotEvent<VoiceSegmentPayload>;

    public record VoiceSegmentPayload
    {
        public required DateTimeOffset Start { get; init; }
        public required DateTimeOffset End { get; init; }
        [JsonIgnore]
        public required byte[] RawAudio;
    }
}
