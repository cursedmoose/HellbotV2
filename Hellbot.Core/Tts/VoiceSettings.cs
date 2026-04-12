namespace Hellbot.Core.TTS
{
    public record VoiceSettings(
        float Stability = 0.33f,
        float SimilarityBoost = 0.66f,
        float Style = 0.75f,
        bool UseSpeakerBoost = false,
        float Speed = 1f
    );
}
