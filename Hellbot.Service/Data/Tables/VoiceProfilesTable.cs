using Dapper;
using Hellbot.Core.TTS;
using System.Text.Json;

namespace Hellbot.Service.Data.Tables
{
    public class VoiceTable(IDbConnectionFactory factory)
    {
        public async Task InsertAsync(Voice voice)
        {
            using var connection = factory.CreateConnection();

            await connection.ExecuteAsync(@"
            INSERT INTO voices (id, name, settings)
            VALUES (@Id, @Name, @Settings)
        ", new
            {
                voice.Id,
                voice.Name,
                Settings = JsonSerializer.Serialize(voice.Settings)
            });
        }

        public async Task<Voice?> Get(string VoiceId)
        {
            using var connection = factory.CreateConnection();
            var voice = await connection.QuerySingleOrDefaultAsync<Voice>(@"
                SELECT id, name, settings
                FROM voices
                WHERE voice_id = @VoiceId
            ", new { VoiceId });

            return voice;
        }

        public async Task<IEnumerable<Voice>> GetAll()
        {
            using var connection = factory.CreateConnection();
            var voices = await connection.QueryAsync<Voice>(@"
                SELECT id, name, settings
                FROM voices
            ");

            return voices;
        }
    }
}
