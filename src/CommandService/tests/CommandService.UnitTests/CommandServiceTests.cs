using AutoMapper;
using Moq;
using CommandService.Application.Contracts.Repos;
using CommandService.Application.Dtos.Command;
using CommandService.Application.Services;
using CommandService.Domain.Entities;
using Xunit;

namespace CommandService.UnitTests;

public class CommandServiceTests
{
    private readonly Mock<ICommandRepo> _repo = new();
    private readonly Mock<IMapper> _mapper = new();

    private Application.Services.CommandService CreateService()
        => new(_repo.Object, _mapper.Object);

    [Fact]
    public void GetCommandsForPlatform_ReturnsMappedDtos()
    {
        var commands = new List<Command>
        {
            new() { Id = 1, HowTo = "h1", CommandLine = "cl1", PlatformId = 5 },
            new() { Id = 2, HowTo = "h2", CommandLine = "cl2", PlatformId = 5 }
        };
        var readDtos = new List<CommandReadDto>
        {
            new() { Id = 1, HowTo = "h1", CommandLine = "cl1", PlatformId = 5 },
            new() { Id = 2, HowTo = "h2", CommandLine = "cl2", PlatformId = 5 }
        };

        _repo.Setup(r => r.GetCommandsForPlatform(5)).Returns(commands);
        _mapper.Setup(m => m.Map<IEnumerable<CommandReadDto>>(commands)).Returns(readDtos);

        var service = CreateService();
        var result = service.GetCommandsForPlatform(5).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("h1", result[0].HowTo);
    }

    [Fact]
    public void GetCommandForPlatform_Existing_ReturnsDto()
    {
        var command = new Command { Id = 7, HowTo = "how", CommandLine = "run", PlatformId = 2 };
        var readDto = new CommandReadDto { Id = 7, HowTo = "how", CommandLine = "run", PlatformId = 2 };

        _repo.Setup(r => r.GetCommand(2, 7)).Returns(command);
        _mapper.Setup(m => m.Map<CommandReadDto>(command)).Returns(readDto);

        var service = CreateService();
        var result = service.GetCommandForPlatform(2, 7);

        Assert.NotNull(result);
        Assert.Equal(readDto.CommandLine, result!.CommandLine);
    }

    [Fact]
    public void CreateCommand_ValidInput_Persists()
    {
        var createDto = new CommandCreateDto { HowTo = "do", CommandLine = "cmd" };
        var command = new Command { Id = 3, HowTo = "do", CommandLine = "cmd", PlatformId = 1 };
        var readDto = new CommandReadDto { Id = 3, HowTo = "do", CommandLine = "cmd", PlatformId = 1 };

        _mapper.Setup(m => m.Map<Command>(createDto)).Returns(command);
        _mapper.Setup(m => m.Map<CommandReadDto>(command)).Returns(readDto);
        _repo.Setup(r => r.CreateCommand(1, command));
        _repo.Setup(r => r.SaveChanges()).Returns(true);

        var service = CreateService();
        var result = service.CreateCommand(1, createDto);

        Assert.NotNull(result);
        Assert.Equal(readDto.Id, result!.Id);
        _repo.Verify(r => r.CreateCommand(1, command), Times.Once);
        _repo.Verify(r => r.SaveChanges(), Times.Once);
    }
}

