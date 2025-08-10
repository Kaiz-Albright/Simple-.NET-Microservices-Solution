using Microsoft.EntityFrameworkCore;
using PlatformService.Domain.Entities;
using PlatformService.Infrastructure.Data;
using PlatformService.Infrastructure.Repositories;
using Xunit;

namespace PlatformService.UnitTests;

public class PlatformRepoTests
{
    [Fact]
    public void CreatePlatform_PersistsPlatform()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new PlatformDbContext(options);
        var repo = new PlatformRepo(context);
        var platform = new Platform
        {
            Name = "Test Platform",
            Publisher = "Test Publisher",
            Cost = "Free"
        };

        repo.CreatePlatform(platform);
        repo.SaveChanges();

        Assert.Single(context.Platforms);
        Assert.Equal("Test Platform", context.Platforms.First().Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreatePlatform_InvalidName_ThrowsArgumentException(string? name)
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new PlatformDbContext(options);
        var repo = new PlatformRepo(context);
        var platform = new Platform
        {
            Name = name!,
            Publisher = "Test Publisher",
            Cost = "Free"
        };

        Assert.Throws<ArgumentException>(() => repo.CreatePlatform(platform));
    }

    [Fact]
    public void GetAllPlatforms_ReturnsOrderedPlatforms()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new PlatformDbContext(options);
        context.Platforms.AddRange(
            new Platform { Name = "P1", Publisher = "Pub1", Cost = "Free" },
            new Platform { Name = "P2", Publisher = "Pub2", Cost = "Free" },
            new Platform { Name = "P3", Publisher = "Pub3", Cost = "Free" }
        );
        context.SaveChanges();
        var repo = new PlatformRepo(context);

        var result = repo.GetAllPlatforms().ToList();

        Assert.Equal(3, result.Count);
        Assert.True(result.SequenceEqual(result.OrderBy(p => p.Id)));
    }

    [Fact]
    public void GetPlatformById_ExistingId_ReturnsPlatform()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new PlatformDbContext(options);
        var platform = new Platform { Name = "P1", Publisher = "Pub1", Cost = "Free" };
        context.Platforms.Add(platform);
        context.SaveChanges();
        var repo = new PlatformRepo(context);

        var result = repo.GetPlatformById(platform.Id);

        Assert.NotNull(result);
        Assert.Equal(platform.Name, result!.Name);
    }

    [Fact]
    public void GetPlatformById_NonExistingId_ReturnsNull()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new PlatformDbContext(options);
        var repo = new PlatformRepo(context);

        var result = repo.GetPlatformById(42);

        Assert.Null(result);
    }
}
