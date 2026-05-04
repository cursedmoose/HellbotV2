using Hellbot.Core.Events;
using Hellbot.Core.Events.Session;

namespace Hellbot.UI.Components.Model
{
    public sealed class ServiceStatusRowDto
    {
        public PlatformSource Platform { get; set; }
        public ConnectionState Status { get; set; }
        public string? Details { get; set; }
        public DateTimeOffset LastChanged { get; set; }
    }
}
