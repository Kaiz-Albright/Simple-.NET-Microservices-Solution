using CommandService.Application.Contracts.Repos;
using CommandService.Domain.Entities;
using CommandService.Infrastructure.Data;

namespace CommandService.Infrastructure.Repositories;

public class PlatformRepo : IPlatformRepo
{
    private readonly CommandDbContext _context;

    public PlatformRepo(CommandDbContext context)
    {
        _context = context;
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform == null)
        {
            throw new ArgumentNullException(nameof(platform), "Platform cannot be null.");
        }

        _context.Platforms.Add(platform);
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
        return _context.Platforms
            .Any(p => p.ExternalID == externalPlatformId);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms
            .OrderBy(p => p.Id)
            .ToList();
    }

    public bool PlatformExists(int platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
