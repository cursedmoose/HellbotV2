using Hellbot.Service.Clients.ElevenLabs;
using Hellbot.Service.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("voice")]
    [ApiController]
    public class VoicesController(VoiceProfilesTable db): ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostVoice(Voice voice)
        {
            var voiceProfile = new VoiceProfile(Voice: voice, Settings: new());
            await db.InsertAsync(voiceProfile);
            return Ok();
        }
    }
}
