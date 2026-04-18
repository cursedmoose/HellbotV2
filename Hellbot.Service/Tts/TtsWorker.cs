using Hellbot.Service.Clients.ElevenLabs;
using Hellbot.Service.Clients.OBS;

namespace Hellbot.Service.Tts
{
    public class TtsWorker(
        ITtsQueue queue,
        IAudioPlayer player,
        ITtsPlaybackGate playbackGate,
        ElevenLabsClient tts,
        ObsClient obs,
        ILogger<TtsWorker> logger) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var evt in queue.DequeueAllAsync(stoppingToken))
            {
                try
                {
                    await playbackGate.WaitAsync(stoppingToken);
                    var audio = await tts.GenerateTts(evt.VoiceId, evt.Message);
                    obs.EnableScene(evt.SceneId);
                    await player.PlayAsync(audio, stoppingToken);
                    obs.DisableScene(evt.SceneId);
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Failed to play TTS event: {Error}", ex.Message);
                }
            }
        }
    }
}
