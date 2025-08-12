using System.Net;
using System.Net.Http.Json;
using PlatformService.Application.Dtos;
using Xunit;

namespace PlatformService.IntegrationTests;

/// <summary>
/// Integration tests for the PlatformsController using the custom API factory.
/// </summary>
public class PlatformsControllerTests : IClassFixture<PlatformServiceApiFactory>
{
    private readonly HttpClient _client;

    public PlatformsControllerTests(PlatformServiceApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPlatforms_ReturnsSeededPlatforms()
    {
        var response = await _client.GetAsync("/api/platforms");
        response.EnsureSuccessStatusCode();
        var platforms = await response.Content.ReadFromJsonAsync<List<PlatformReadDto>>();
        Assert.NotNull(platforms);
    }

    [Fact]
    public async Task CreatePlatform_PersistsPlatform()
    {
        var before = await _client.GetFromJsonAsync<List<PlatformReadDto>>("/api/platforms") ?? new();
        var createDto = new PlatformCreateDto { Name = "Test", Publisher = "Tester", Cost = "Free" };
        var response = await _client.PostAsJsonAsync("/api/platforms", createDto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var after = await _client.GetFromJsonAsync<List<PlatformReadDto>>("/api/platforms") ?? new();
        Assert.Equal(before.Count + 1, after.Count);
    }
}
