using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Models;
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
}

