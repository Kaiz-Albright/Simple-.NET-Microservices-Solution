using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlatformService.Controllers;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.Profiles;
using PlatformService.Services;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Tests;

public class PlatformsControllerTests
{
    private PlatformsController CreateController(
        Mock<IPlatformRepo> repoMock,
        Mock<ICommandDataClient> clientMock)
    {
        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<PlatformsProfile>());
        var mapper = mapperConfig.CreateMapper();
        var service = new PlatformService.Services.PlatformService(
            repoMock.Object,
            mapper,
            clientMock.Object);
        return new PlatformsController(service);
    }

    [Fact]
    public void GetPlatforms_ReturnsOkWithList()
    {
        // Arrange
        var repoMock = new Mock<IPlatformRepo>();
        var clientMock = new Mock<ICommandDataClient>();
        repoMock.Setup(r => r.GetAllPlatforms()).Returns(new List<Platform>
        {
            new Platform { Id = 1, Name = "P1", Publisher = "Pub1", Cost = "Free" },
            new Platform { Id = 2, Name = "P2", Publisher = "Pub2", Cost = "Free" }
        });
        var controller = CreateController(repoMock, clientMock);

        // Act
        var result = controller.GetPlatforms();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var platforms = Assert.IsAssignableFrom<IEnumerable<PlatformReadDto>>(okResult.Value);
        Assert.Equal(2, platforms.Count());
    }

    [Fact]
    public void GetPlatformById_NotFound_ReturnsNotFound()
    {
        // Arrange
        var repoMock = new Mock<IPlatformRepo>();
        var clientMock = new Mock<ICommandDataClient>();
        repoMock.Setup(r => r.GetPlatformById(It.IsAny<int>())).Returns((Platform?)null);
        var controller = CreateController(repoMock, clientMock);

        // Act
        var result = controller.GetPlatformById(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetPlatformById_Found_ReturnsOk()
    {
        // Arrange
        var repoMock = new Mock<IPlatformRepo>();
        var clientMock = new Mock<ICommandDataClient>();
        var platform = new Platform { Id = 1, Name = "P1", Publisher = "Pub1", Cost = "Free" };
        repoMock.Setup(r => r.GetPlatformById(1)).Returns(platform);
        var controller = CreateController(repoMock, clientMock);

        // Act
        var result = controller.GetPlatformById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<PlatformReadDto>(okResult.Value);
        Assert.Equal(platform.Id, dto.Id);
    }

    [Fact]
    public async Task CreatePlatform_Valid_ReturnsCreatedAndCallsClient()
    {
        // Arrange
        var repoMock = new Mock<IPlatformRepo>();
        var clientMock = new Mock<ICommandDataClient>();
        repoMock.Setup(r => r.CreatePlatform(It.IsAny<Platform>()))
                .Callback<Platform>(p => p.Id = 1);
        repoMock.Setup(r => r.SaveChanges()).Returns(true);
        clientMock.Setup(c => c.SendPlatformToCommand(It.IsAny<PlatformReadDto>()))
                  .Returns(Task.CompletedTask);
        var controller = CreateController(repoMock, clientMock);
        var dto = new PlatformCreateDto { Name = "P1", Publisher = "Pub1", Cost = "Free" };

        // Act
        var result = await controller.CreatePlatform(dto);

        // Assert
        var createdAtRoute = Assert.IsType<CreatedAtRouteResult>(result.Result);
        Assert.Equal(nameof(PlatformsController.GetPlatformById), createdAtRoute.RouteName);
        var readDto = Assert.IsType<PlatformReadDto>(createdAtRoute.Value);
        Assert.Equal(1, readDto.Id);
        clientMock.Verify(c => c.SendPlatformToCommand(It.IsAny<PlatformReadDto>()), Times.Once);
    }

    [Fact]
    public async Task CreatePlatform_RepositoryThrows_ReturnsServerError()
    {
        // Arrange
        var repoMock = new Mock<IPlatformRepo>();
        var clientMock = new Mock<ICommandDataClient>();
        repoMock.Setup(r => r.CreatePlatform(It.IsAny<Platform>()))
                .Throws(new Exception("Database error"));
        var controller = CreateController(repoMock, clientMock);
        var dto = new PlatformCreateDto { Name = "P1", Publisher = "Pub1", Cost = "Free" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => controller.CreatePlatform(dto));
    }
}
