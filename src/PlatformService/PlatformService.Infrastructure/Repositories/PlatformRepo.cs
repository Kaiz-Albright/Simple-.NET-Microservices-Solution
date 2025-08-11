using PlatformService.Application.Repositories;
using PlatformService.Domain.Entities;
using PlatformService.Infrastructure.Data;

namespace PlatformService.Infrastructure.Repositories;

public class PlatformRepo : IPlatformRepo
{
    private readonly PlatformDbContext _context;

    public PlatformRepo(PlatformDbContext context)
    {
        _context = context;
    }

    public void CreatePlatform(Platform platform)
    {
        if (string.IsNullOrWhiteSpace(platform.Name))
        {
            throw new ArgumentException("Platform name cannot be null or empty.", nameof(platform));
        }

        _context.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.OrderBy(p => p.Id).ToList();
    }

    public Platform? GetPlatformById(int id)
    {
        return _context.Platforms.FirstOrDefault(p => p.Id == id);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
