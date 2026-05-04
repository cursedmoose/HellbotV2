using System.Text.Json;
using Hellbot.Core.Events;

namespace Hellbot.UI.Components.Model
{
    public class EventViewModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = "";
        public DateTimeOffset Timestamp { get; set; }
        public EventSourceViewModel? Source { get; set; }
        public JsonElement Data { get; set; }
    }

    public sealed class EventSourceViewModel
    {
        public PlatformSource Platform { get; set; }
        public string? Channel { get; set; }
    }
}
