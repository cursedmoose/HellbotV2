using Hellbot.Core.TTS;
using Hellbot.Service.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("voice")]
    [ApiController]
    public class VoicesController(VoiceTable db): ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostVoice(string Id, string Name)
        {
            var voiceProfile = new Voice(Id, Name, Settings: new());
            await db.InsertAsync(voiceProfile);
            return Ok();
        }
    }
}
