namespace Hellbot.Service.Tts
{
    public interface IAudioPlayer
    {
        Task PlayAsync(byte[] audio, CancellationToken ct);
    }
}
