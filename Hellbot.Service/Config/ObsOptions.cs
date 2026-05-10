using Hellbot.Core.Scenes;

namespace Hellbot.Service.Config
{
    public class ObsOptions
    {
        public string WebsocketUrl { get; set; } = default!;
        // Use '=' in JSON keys; ':' breaks IConfiguration paths. Runtime ids may still use ':' via ObsClient lookup.
        public Dictionary<string, SceneItem> Scenes { get; set; } = new();
    }
}
