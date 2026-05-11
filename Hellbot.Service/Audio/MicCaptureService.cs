using Hellbot.Core.Events;
using Hellbot.Core.Events.Audio;
using NAudio.Wave;

namespace Hellbot.Service.Audio
{
    public class MicCaptureService(IEventBus bus, ILogger<MicCaptureService> logger) : BackgroundService
    {
        private WaveInEvent? _waveIn;
        private readonly WaveFormat _targetFormat = new WaveFormat(16000, 16, 1);
        private readonly EventSource _source = EventSource.Internal with { Channel = "MicCaptureService" };
        private const int Threshold = 500; // tweak later
        private DateTimeOffset? _speechStart;
        private DateTimeOffset _lastVoiceDetected;
        private static readonly TimeSpan SilenceGrace = TimeSpan.FromMilliseconds(750);
        private bool IsSpeaking { get { return _speechStart is not null; } }
        private readonly List<byte> _audioBuffer = [];
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LogAvailableInputDevices();

            _waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(16000, 1)
            };
            logger.LogDebug("Device WaveFormat: {Format}", _waveIn.WaveFormat);

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
            var now = DateTimeOffset.UtcNow;

            if (IsSpeaking)
            {
                _audioBuffer.AddRange(e.Buffer.AsSpan(0, e.BytesRecorded).ToArray());
            }

            bool hasVoice = DetectVoice(e);

            if (hasVoice)
            {
                _lastVoiceDetected = now;

                if (!IsSpeaking)
                {
                    _speechStart = now;
                    _audioBuffer.Clear();

                    PublishSpeechStarted(now);
                }
            }
            else if (IsSpeaking)
            {
                // don't end immediately—wait for silence window
                if (now - _lastVoiceDetected > SilenceGrace)
                {
                    var duration = now - _speechStart!.Value;
                    PublishVoiceSegment(now);

                    _speechStart = null;
                    _audioBuffer.Clear();

                    PublishSpeechEnded(duration);
                }
            }
        }

        private bool DetectVoice(WaveInEventArgs e)
        {
            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                short sample = BitConverter.ToInt16(e.Buffer, i);
                if (Math.Abs(sample) > Threshold)
                {
                    return true;
                }
            }

            return false;
        }

        private void PublishSpeechStarted(DateTimeOffset now)
        {
            bus.Publish(new SpeechStarted
            {
                Data = new(),
                Source = _source,
            });
        }

        private void PublishSpeechEnded(TimeSpan duration)
        {
            bus.Publish(new SpeechEnded
            {
                Data = new()
                {
                    Duration = duration
                },
                Source = _source,
            });
        }

        public void PublishVoiceSegment(DateTimeOffset now)
        {
            logger.LogDebug("Segment bytes: {Bytes}", _audioBuffer.Count);
            bus.Publish(new VoiceSegmentCaptured
            {
                Data = new()
                {
                    Start = _speechStart!.Value,
                    End = now,
                    RawAudio = [.. _audioBuffer]
                },
                Source = _source
            });
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
                logger.LogDebug("Input Device {Index}: {Name}", i, caps.ProductName);
            }
        }
    }
}
