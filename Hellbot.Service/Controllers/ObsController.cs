using Hellbot.Service.Clients.OBS;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObsController(ObsClient obs) : ControllerBase
    {
        [HttpGet("scenes")]
        public async Task<object> GetSceneList(string? scene)
        {
            if (string.IsNullOrEmpty(scene)) {
                return obs.API.GetSceneList();
            }
            else
            {
                try
                {
                    return obs.API.GetSceneItemList(scene);
                } catch (Exception)
                {
                    return $"Invalid Scene Id {scene}";
                }
            }
        }

        [HttpPost("scenes/{scene}/enable")]
        public async Task<IActionResult> EnableScene(string scene)
        {
            obs.EnableScene(scene);
            return Ok();
        }

        [HttpPost("scenes/{scene}/disable")]
        public async Task<IActionResult> DisableScene(string scene)
        {
            obs.DisableScene(scene);
            return Ok();
        }

    }
}
