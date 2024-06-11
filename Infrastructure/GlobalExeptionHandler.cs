using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace SecurityApi.Infrastructure
{
    public class GlobalExeptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExeptionHandler> _logger;
        public GlobalExeptionHandler(ILogger<GlobalExeptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);

            var details = new ProblemDetails()
            {
                Detail = $"API Error {exception.Message}",
                Instance = "API",
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "API Error",
                Type = "Server Error"
            };

            var response = JsonSerializer.Serialize(details);
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(response, cancellationToken);
            return true;
        }
    }
}
