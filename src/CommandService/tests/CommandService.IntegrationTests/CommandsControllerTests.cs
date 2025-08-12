using System.Net;
using System.Net.Http.Json;
using CommandService.Application.Dtos.Command;
using Xunit;

namespace CommandService.IntegrationTests;

/// <summary>
/// Integration tests for the CommandsController using the custom API factory.
/// </summary>
public class CommandsControllerTests : IClassFixture<CommandServiceApiFactory>
{
    private readonly HttpClient _client;

    public CommandsControllerTests(CommandServiceApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCommandsForPlatform_ReturnsSeededCommands()
    {
        var response = await _client.GetAsync("/api/c/platforms/1/commands");
        response.EnsureSuccessStatusCode();
        var commands = await response.Content.ReadFromJsonAsync<List<CommandReadDto>>();
        Assert.NotNull(commands);
        Assert.NotEmpty(commands!);
    }

    [Fact]
    public async Task CreateCommand_PersistsCommand()
    {
        var before = await _client.GetFromJsonAsync<List<CommandReadDto>>("/api/c/platforms/1/commands") ?? new();
        var createDto = new CommandCreateDto { HowTo = "test", CommandLine = "run" };
        var response = await _client.PostAsJsonAsync("/api/c/platforms/1/commands", createDto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var after = await _client.GetFromJsonAsync<List<CommandReadDto>>("/api/c/platforms/1/commands") ?? new();
        Assert.Equal(before.Count + 1, after.Count);
    }
}
