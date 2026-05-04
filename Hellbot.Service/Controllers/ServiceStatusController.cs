using Hellbot.Service.Status;
using Microsoft.AspNetCore.Mvc;

namespace Hellbot.Service.Controllers
{
    [Route("service-status")]
    [ApiController]
    public sealed class ServiceStatusController(ServiceStatusProvider status) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(status.GetWebsocketProducerStatuses());
    }
}
