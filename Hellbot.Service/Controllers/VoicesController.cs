using Hellbot.Service.Clients.ElevenLabs;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("voices")]
    [ApiController]
    public class VoicesController(ElevenLabsClient client): ControllerBase
    {
        [HttpGet]
        public IActionResult GetVoices()
        {
            return Ok(client.Voices);
        }

        [HttpGet("{voiceId}")]
        public IActionResult GetVoice(string voiceId)
        {
            if (client.Voices.TryGetValue(voiceId, out var voice))
            {
                return Ok(voice);
            }
            return NotFound();
        }
    }
}
