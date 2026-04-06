using Hellbot.Service.Clients.ElevenLabs;

namespace Hellbot.Service.Tts
{
    public class TtsWorker(ITtsQueue queue, IAudioPlayer player, ElevenLabsClient tts, ILogger<TtsWorker> logger) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var evt in queue.DequeueAllAsync(stoppingToken))
            {
                try
                {
                    var audio = await tts.GenerateTts(evt.Data.VoiceId, evt.Data.Text);
                    await player.PlayAsync(audio, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Failed to play TTS event: {Error}", ex.Message);
                }
            }
        }
    }
}
