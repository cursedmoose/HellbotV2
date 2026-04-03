using Dapper;
using Hellbot.Core.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hellbot.Service.Data
{
    public class EventTable(IDbConnectionFactory factory)
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public async Task InsertAsync(IHellbotEvent evt)
        {
            using var connection = factory.CreateConnection();

            var eventType = evt.GetType();

            var eventJson = JsonSerializer.Serialize(evt, eventType, _jsonOptions);

            await connection.ExecuteAsync(@"
            INSERT INTO events (id, timestamp, platform, event_type, metadata)
            VALUES (@Id, @Timestamp, @Platform, @EventType, @Metadata)
        ", new
            {
                Id = evt.Id,
                Timestamp = evt.Timestamp,
                Platform = PlatformSource.GetName(evt.Source.Platform),
                EventType = eventType.Name,
                Metadata = eventJson
            });
        }
    }
}
