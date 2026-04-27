namespace Hellbot.Service.Config
{
    public class WhisperOptions
    {
        public string ModelPath { get; set; } = "models/ggml-base.bin";
        public string Language { get; set; } = "en";
    }
}
