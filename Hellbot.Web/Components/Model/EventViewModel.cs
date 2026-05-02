using System.Text.Json;

namespace Hellbot.UI.Components.Model
{
    public class EventViewModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = "";
        public DateTimeOffset Timestamp { get; set; }
        public JsonElement Data { get; set; }
    }
}
