using System.Threading.Tasks;
using KX.StorageService;
using KX.StorageService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace KX.Gateway.Controllers
{
    [Route("data")]
    [ApiController]
    public class DataController : Controller
    {
        private readonly ILogger<DataController> logger;
        private readonly IDataProvider dataProvider;

        public DataController(ILogger<DataController> logger, IDataProvider dataProvider)
        {
            this.logger = logger;
            this.dataProvider = dataProvider;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            return Ok(JsonConvert.SerializeObject(await dataProvider.GetPartitions()));
        }

        [HttpGet("{partitionKey}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string partitionKey)
        {
            if(!await dataProvider.TryGetPartition(partitionKey, out var partition))
            {
                return NotFound();
            }

            return Ok(JsonConvert.SerializeObject(partition.Keys));
        }

        [HttpGet("{partitionKey}/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string partitionKey, string id)
        {
            if (!await dataProvider.TryGetPartition(partitionKey, out var partition))
            {
                return NotFound();
            }

            if (!partition.TryGetValue(id, out var data))
            {
                return NotFound();
            }

            return Ok(JsonConvert.SerializeObject(data));
        }
    }
}
