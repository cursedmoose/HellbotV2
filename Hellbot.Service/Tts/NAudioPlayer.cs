using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Hellbot.Service.Tts
{
    public class NAudioPlayer: IAudioPlayer
    {
        private readonly MMDevice _device;

        public NAudioPlayer()
        {
            var enumerator = new MMDeviceEnumerator();
            _device = enumerator
                .EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                .First();
                //.First(d => d.FriendlyName.Contains("CABLE Input"));
        }

        public async Task PlayAsync(byte[] audio, CancellationToken ct)
        {
            using var ms = new MemoryStream(audio);
            using var reader = new Mp3FileReader(ms);

            using var output = new WasapiOut(_device, AudioClientShareMode.Shared, false, 200);
            output.Init(reader);
            output.Play();

            while (output.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(50, ct);
            }
        }
    }
}
