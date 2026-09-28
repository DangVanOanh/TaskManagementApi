using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagement.Api.Data;

namespace TaskManagement.Tests.Api;

public class TasksApiValidationTests : IClassFixture<TasksApiValidationTests.ApiFactory>
{
    private readonly HttpClient _client;

    public TasksApiValidationTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_MissingTitle_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_EmptyTitle_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { Title = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WhitespaceOnlyTitle_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { Title = "   " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ValidTitle_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { Title = "Write report" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Put_EmptyTitle_Returns400()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", new { Title = "Original title" });
        using var created = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync());
        var createdId = created.RootElement.GetProperty("id").GetInt32();

        var putResponse = await _client.PutAsJsonAsync($"/api/tasks/{createdId}", new { Title = "" });

        Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
    }

    public class ApiFactory : WebApplicationFactory<Program>
    {
        // In-memory SQLite keeps its schema only while this connection stays open,
        // so tests never touch the real taskmanagement.db file.
        private readonly SqliteConnection _connection = new("DataSource=:memory:");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _connection.Open();

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection.Dispose();
            }
        }
    }
}
