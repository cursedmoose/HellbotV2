using NAudio.Utils;
using NAudio.Wave;

namespace Hellbot.Service.Audio
{
    public static class AudioConverter
    {
        public static MemoryStream ToWavStream(byte[] rawAudio)
        {
            var memoryStream = new MemoryStream();

            var waveFormat = new WaveFormat(16000, 16, 1);

            using (var writer = new WaveFileWriter(new IgnoreDisposeStream(memoryStream), waveFormat))
            {
                writer.Write(rawAudio, 0, rawAudio.Length);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public static List<byte> Resample(WaveFormat sourceFormat, WaveFormat targetFormat, WaveInEventArgs e)
        {

            using var sourceStream = new RawSourceWaveStream(
                e.Buffer, 0, e.BytesRecorded, sourceFormat);

            using var resampler = new MediaFoundationResampler(sourceStream, targetFormat)
            {
                ResamplerQuality = 60
            };

            var convertedBuffer = new byte[4096];
            int bytesRead;
            List<byte> _audioBuffer = [];

            while ((bytesRead = resampler.Read(convertedBuffer, 0, convertedBuffer.Length)) > 0)
            {
                _audioBuffer.AddRange(convertedBuffer.AsSpan(0, bytesRead).ToArray());
            }

            return _audioBuffer;
        }
    }
}
