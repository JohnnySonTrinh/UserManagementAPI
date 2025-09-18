using System.Text.Json;
using Microsoft.OpenApi.Models;
using UserManagementAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers + System.Text.Json camelCase
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.WriteIndented = false;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1",
        Description = "Simple User Management API with in-memory storage"
    });
});

// DI
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/", () => Results.Text("User Management API"));
app.MapControllers();

app.Run();

app.Run();
