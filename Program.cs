using System.Text.Json;
using Microsoft.OpenApi.Models;
using UserManagementAPI.Repositories;
using UserManagementAPI.Middleware;

// Create the application builder
var builder = WebApplication.CreateBuilder(args);


// Add controllers and configure JSON serialization to use camelCase
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.WriteIndented = false;
    });


// Add Swagger/OpenAPI support and document the API password header
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1",
        Description = "Simple User Management API with in-memory storage"
    });

    // Add X-Api-Password header to all endpoints for documentation and testing
    c.AddSecurityDefinition("ApiPassword", new OpenApiSecurityScheme
    {
        Name = "X-Api-Password",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "API password required for protected endpoints."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiPassword"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Register the in-memory user repository for dependency injection
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

// Build the application
var app = builder.Build();

// Password-protected API middleware
// Only requests with X-Api-Password: 12345 are allowed, except for /swagger and /
// This is a simple demo authentication for development/testing purposes
app.Use(async (context, next) =>
{
    var password = context.Request.Headers["X-Api-Password"].FirstOrDefault();
    if (context.Request.Path.StartsWithSegments("/swagger") || context.Request.Path == "/")
    {
        await next();
        return;
    }
    if (password == "12345")
    {
        await next();
    }
    else
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
    }
});

// Enable Swagger UI for API documentation and testing
if (app.Environment.IsDevelopment())
{
    // Swagger is public in Development
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Require authentication for Swagger in Production (not used in this simple demo)
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        await next();
    });
}


// Error handling middleware for consistent error responses
app.UseMiddleware<ErrorHandlingMiddleware>();

// Request/response logging middleware for diagnostics
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Authorization middleware (not used in this simple demo, but present for extensibility)
app.UseAuthorization();

// Root endpoint for health check or welcome message
app.MapGet("/", () => Results.Text("User Management API"));

// Map controller endpoints
app.MapControllers();

// Start the application
app.Run();
