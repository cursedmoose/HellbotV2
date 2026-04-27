using Hellbot.Core.Events;
using Hellbot.Core.Events.Audio;
using NAudio.Wave;

namespace Hellbot.Service.Audio
{
    public class MicCaptureService(IEventBus bus, ILogger<MicCaptureService> logger) : BackgroundService
    {
        private WaveInEvent? _waveIn;
        private EventSource _source = EventSource.Internal with { Channel = "MicCaptureService" };
        private const int Threshold = 500; // tweak later
        private DateTimeOffset? _speechStart;
        private bool IsSpeaking { get { return _speechStart is not null; } }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogAvailableInputDevices();

            _waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(16000, 1)
            };

            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;

            _waveIn.StartRecording();

            logger.LogInformation("🎤 Mic capture started");

            // Stop cleanly when app shuts down
            stoppingToken.Register(() =>
            {
                logger.LogInformation("Stopping mic capture...");
                _waveIn?.StopRecording();
            });

            return Task.CompletedTask;
        }



        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            bool hasVoice = false;

            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                short sample = BitConverter.ToInt16(e.Buffer, i);
                if (Math.Abs(sample) > Threshold)
                {
                    hasVoice = true;
                    break;
                }
            }

            if (hasVoice && !IsSpeaking)
            {
                _speechStart = DateTimeOffset.UtcNow;
                bus.Publish(new SpeechStarted { 
                    Data = new(),
                    Source = _source,
                });
            }
            else if (!hasVoice && IsSpeaking)
            {
                var duration = DateTimeOffset.UtcNow - (_speechStart ?? DateTimeOffset.UtcNow);
                _speechStart = null;
                bus.Publish(new SpeechEnded
                {
                    Data = new()
                    {
                        Duration = duration
                    },
                    Source = _source,
                });
            }
        }

        private void OnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            logger.LogInformation("🛑 Mic capture stopped");

            if (e.Exception != null)
            {
                logger.LogError(e.Exception, "Mic capture error");
            }

            _waveIn?.Dispose();
            _waveIn = null;
        }

        private void LogAvailableInputDevices()
        {
            for (int i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var caps = WaveInEvent.GetCapabilities(i);
                logger.LogInformation("Input Device {Index}: {Name}", i, caps.ProductName);
            }
        }
    }
}
