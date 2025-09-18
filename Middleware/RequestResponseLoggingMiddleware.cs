using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace UserManagementAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            // Read X-Correlation-Id from request or generate one
            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }
            context.Response.Headers["X-Correlation-Id"] = correlationId;

            string userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            string method = context.Request.Method;
            string path = context.Request.Path;
            string query = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";

            await _next(context);

            sw.Stop();
            int statusCode = context.Response.StatusCode;
            _logger.LogInformation("{CorrelationId} {Method} {Path}{Query} {StatusCode} {ElapsedMs}ms User:{UserId}",
                correlationId, method, path, query, statusCode, sw.ElapsedMilliseconds, userId);
        }
    }
}
