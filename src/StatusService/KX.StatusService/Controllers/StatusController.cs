using System.Threading.Tasks;
using KX.StatusService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KX.StatusService.Controllers
{
    [Route("status")]
    [ApiController]
    public class StatusController : Controller
    {
        private readonly ILogger<StatusController> logger;
        private readonly IStatusService statusService;

        public StatusController(ILogger<StatusController> logger, IStatusService statusService)
        {
            this.logger = logger;
            this.statusService = statusService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery(Name = "id")] string id, [FromQuery(Name = "appName")] string appName)
        {
            var result = await statusService.GetContainerStatuses(id, appName);
            return Ok(JsonConvert.SerializeObject(result));
        }
    }
}
