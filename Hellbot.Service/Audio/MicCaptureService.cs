using NAudio.Wave;

namespace Hellbot.Service.Audio
{
    public class MicCaptureService(ILogger<MicCaptureService> logger) : BackgroundService
    {
        private WaveInEvent? _waveIn;

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

        private bool _isSpeaking = false;
        private const int Threshold = 500; // tweak later

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

            if (hasVoice && !_isSpeaking)
            {
                _isSpeaking = true;
                logger.LogInformation("🗣️ Speech started");
            }
            else if (!hasVoice && _isSpeaking)
            {
                _isSpeaking = false;
                logger.LogInformation("🤫 Speech ended");
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
