using AutoMapper;
using Moq;
using PlatformService.Application.AppServices.Interfaces;
using PlatformService.Application.Dtos;
using PlatformService.Application.Services;
using PlatformService.Domain.Entities;
using PlatformService.Application.Contracts.Repos;
using PlatformService.Application.Contracts.Services;
using Xunit;

namespace PlatformService.UnitTests;

public class PlatformServiceTests
{
    private readonly Mock<IPlatformRepo> _repo = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ICommandDataClient> _commandClient = new();
    private readonly Mock<IMessageBusClient> _messageBus = new();

    private Application.Services.PlatformService CreateService()
        => new(_repo.Object, _mapper.Object, _commandClient.Object, _messageBus.Object);

    [Fact]
    public void GetAllPlatforms_ReturnsMappedDtos()
    {
        var platforms = new List<Platform>
        {
            new() { Id = 1, Name = "P1", Publisher = "Pub1", Cost = "Free" },
            new() { Id = 2, Name = "P2", Publisher = "Pub2", Cost = "Free" }
        };
        var readDtos = new List<PlatformReadDto>
        {
            new() { Id = 1, Name = "P1", Publisher = "Pub1", Cost = "Free" },
            new() { Id = 2, Name = "P2", Publisher = "Pub2", Cost = "Free" }
        };

        _repo.Setup(r => r.GetAllPlatforms()).Returns(platforms);
        _mapper.Setup(m => m.Map<IEnumerable<PlatformReadDto>>(platforms)).Returns(readDtos);

        var service = CreateService();
        var result = service.GetAllPlatforms().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("P1", result[0].Name);
    }

    [Fact]
    public void GetPlatformById_Existing_ReturnsDto()
    {
        var platform = new Platform { Id = 7, Name = "Test", Publisher = "Pub", Cost = "Free" };
        var readDto = new PlatformReadDto { Id = 7, Name = "Test", Publisher = "Pub", Cost = "Free" };

        _repo.Setup(r => r.GetPlatformById(7)).Returns(platform);
        _mapper.Setup(m => m.Map<PlatformReadDto>(platform)).Returns(readDto);

        var service = CreateService();
        var result = service.GetPlatformById(7);

        Assert.NotNull(result);
        Assert.Equal(readDto.Name, result!.Name);
    }

    [Fact]
    public async Task CreatePlatformAsync_ValidInput_PersistsAndPublishes()
    {
        var createDto = new PlatformCreateDto { Name = "New", Publisher = "Me", Cost = "Free" };
        var platform = new Platform { Id = 10, Name = "New", Publisher = "Me", Cost = "Free" };
        var readDto = new PlatformReadDto { Id = 10, Name = "New", Publisher = "Me", Cost = "Free" };
        var publishedDto = new PlatformPublishedDto { Id = 10, Name = "New", Event = "Platform_Published" };

        _mapper.Setup(m => m.Map<Platform>(createDto)).Returns(platform);
        _mapper.Setup(m => m.Map<PlatformReadDto>(platform)).Returns(readDto);
        _mapper.Setup(m => m.Map<PlatformPublishedDto>(readDto)).Returns(publishedDto);
        _repo.Setup(r => r.CreatePlatform(platform));
        _repo.Setup(r => r.SaveChanges()).Returns(true);
        _commandClient.Setup(c => c.SendPlatformToCommand(readDto)).Returns(Task.CompletedTask);
        _messageBus.Setup(m => m.PublishNewPlatform(It.Is<PlatformPublishedDto>(p => p.Id == 10))).Returns(Task.CompletedTask);

        var service = CreateService();
        var result = await service.CreatePlatformAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(readDto.Id, result!.Id);
        _repo.Verify(r => r.CreatePlatform(platform), Times.Once);
        _repo.Verify(r => r.SaveChanges(), Times.Once);
        _commandClient.Verify(c => c.SendPlatformToCommand(readDto), Times.Once);
        _messageBus.Verify(m => m.PublishNewPlatform(It.Is<PlatformPublishedDto>(p => p.Id == 10 && p.Event == "Platform_Published")), Times.Once);
    }
}

