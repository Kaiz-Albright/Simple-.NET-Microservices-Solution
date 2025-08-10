using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Data.Repos;
using PlatformService.Models;
using System.Linq;
using Xunit;

namespace PlatformService.Tests;

public class PlatformRepoTests
{
    [Fact]
    public void CreatePlatform_PersistsPlatform()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new AppDbContext(options);
        var repo = new PlatformRepo(context);
        var platform = new Platform
        {
            Name = "Test Platform",
            Publisher = "Test Publisher",
            Cost = "Free"
        };

        // Act
        repo.CreatePlatform(platform);
        repo.SaveChanges();

        // Assert
        Assert.Single(context.Platforms);
        Assert.Equal("Test Platform", context.Platforms.First().Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreatePlatform_InvalidName_ThrowsArgumentException(string? name)
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new AppDbContext(options);
        var repo = new PlatformRepo(context);
        var platform = new Platform
        {
            Name = name!,
            Publisher = "Test Publisher",
            Cost = "Free"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => repo.CreatePlatform(platform));
    }

    [Fact]
    public void GetAllPlatforms_ReturnsOrderedPlatforms()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new AppDbContext(options);
        context.Platforms.AddRange(
            new Platform { Name = "P1", Publisher = "Pub1", Cost = "Free" },
            new Platform { Name = "P2", Publisher = "Pub2", Cost = "Free" },
            new Platform { Name = "P3", Publisher = "Pub3", Cost = "Free" }
        );
        context.SaveChanges();
        var repo = new PlatformRepo(context);

        // Act
        var result = repo.GetAllPlatforms().ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.True(result.SequenceEqual(result.OrderBy(p => p.Id)));
    }

    [Fact]
    public void GetPlatformById_ExistingId_ReturnsPlatform()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new AppDbContext(options);
        var platform = new Platform { Name = "P1", Publisher = "Pub1", Cost = "Free" };
        context.Platforms.Add(platform);
        context.SaveChanges();
        var repo = new PlatformRepo(context);

        // Act
        var result = repo.GetPlatformById(platform.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platform.Name, result!.Name);
    }

    [Fact]
    public void GetPlatformById_NonExistingId_ReturnsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        using var context = new AppDbContext(options);
        var repo = new PlatformRepo(context);

        // Act
        var result = repo.GetPlatformById(42);

        // Assert
        Assert.Null(result);
    }
}

