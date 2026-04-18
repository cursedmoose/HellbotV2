namespace Hellbot.Service.Tts
{
    public interface ITtsPlaybackGate
    {
        Task WaitAsync(CancellationToken ct);
        void Pause(string reason);
        void Resume(string reason);
    }
}
