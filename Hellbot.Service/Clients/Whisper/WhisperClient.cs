using Hellbot.Service.Config;
using Microsoft.Extensions.Options;
using System.Text;
using Whisper.net;

namespace Hellbot.Service.Clients.Whisper
{
    public class WhisperClient(IOptions<WhisperOptions> options)
    {
        private readonly WhisperFactory _factory = WhisperFactory.FromPath(options.Value.ModelPath);

        public async Task<string> TranscribeAsync(Stream audioStream)
        {
            var builder = _factory.CreateBuilder()
                .WithLanguage("en")
                .Build();

            var text = new StringBuilder();

            await foreach (var segment in builder.ProcessAsync(audioStream))
            {
                text.Append(segment.Text);
            }

            return text.ToString();
        }
    }
}
