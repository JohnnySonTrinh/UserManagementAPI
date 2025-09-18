using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace UserManagementAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                int statusCode = 500;
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json; charset=utf-8";

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "An unexpected error occurred.",
                    Type = "https://httpstatuses.com/500",
                    Instance = context.Request.Path
                };
                problem.Extensions["traceId"] = context.TraceIdentifier;
                if (_env.IsDevelopment())
                {
                    problem.Detail = ex.ToString();
                }
                var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await context.Response.WriteAsync(json);
            }
        }
    }
}
