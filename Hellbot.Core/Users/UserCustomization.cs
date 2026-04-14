using Hellbot.Core.TTS;

namespace Hellbot.Core.Users
{
    public class UserCustomizationSet
    {
        public string? VoiceId;
        public VoiceSettings? VoiceSettings;
        public string? SceneId;
    }
    public record UserCustomization
    {
        public required CustomizationType Type { get; init; }
        public required string Value { get; init; }
    }

    public enum CustomizationType
    {
        VoiceId,
        VoiceSettings,
        SceneId,
    }
}
