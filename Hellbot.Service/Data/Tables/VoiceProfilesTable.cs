using Dapper;
using Hellbot.Service.Clients.ElevenLabs;
using System.Text.Json;

namespace Hellbot.Service.Data.Tables
{
    public class VoiceProfilesTable(IDbConnectionFactory factory)
    {
        public async Task InsertAsync(VoiceProfile profile)
        {
            using var connection = factory.CreateConnection();

            await connection.ExecuteAsync(@"
            INSERT INTO voices (id, name, settings)
            VALUES (@Id, @Name, @Settings)
        ", new
            {
                Id = profile.Voice.VoiceId,
                Name = profile.Voice.VoiceName,
                Settings = JsonSerializer.Serialize(profile.Settings)
            });
        }

        public async Task<VoiceProfile?> Get(string VoiceId)
        {
            using var connection = factory.CreateConnection();
            var voice = await connection.QuerySingleOrDefaultAsync<VoiceProfile>(@"
                SELECT id, name, settings
                FROM voices
                WHERE voice_id = @VoiceId
            ", new { VoiceId });

            return voice;
        }

        public async Task<IEnumerable<VoiceProfile>> GetAll()
        {
            using var connection = factory.CreateConnection();
            var voices = await connection.QueryAsync<VoiceProfile>(@"
                SELECT id, name, settings
                FROM voices
            ");

            return voices;
        }
    }
}
