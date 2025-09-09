using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KX.StatusService.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("/Error")]
        public IActionResult HttpStatusCodeHandler()
        {
            Guid errorGuid = Guid.NewGuid();

            string errorMessage = HttpContext.Response.StatusCode switch
            {
                int i when i == 404 => "Resource could not be found.",
                _ => $"Unexpected Error. Contact an administrator with the following error Guid: {errorGuid}"
            };

            var exHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            
            logger.LogError(exHandlerFeature.Error, $"Guid: {errorGuid.ToString()}");

            return new ObjectResult(errorMessage);
        }
    }
}
