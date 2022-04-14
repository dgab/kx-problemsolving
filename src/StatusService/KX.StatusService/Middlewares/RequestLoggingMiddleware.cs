using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KX.StatusService.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                // Don't remove. Without the if statement there would be an unnecessary string interpolation with each request
                if (_logger.IsEnabled(LogLevel.Information) && context.Request?.Path.Value != "/healthz") 
                {
                    _logger.LogInformation(
                         "Request {method} {url}{query} => {statusCode}",
                         context.Request?.Method,
                         context.Request?.Path.Value,
                         context.Request?.QueryString.Value,
                         context.Response?.StatusCode);
                }
            }
        }
    }
}
