using Company.Platform.Application.Repos;
using Company.Platform.Domain.Models;

namespace Company.Platform.Infrastructure.Data.Repos;

public class PlatformRepo : IPlatformRepo
{
    private readonly AppDbContext _context;

    public PlatformRepo(AppDbContext context)
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

    public IEnumerable<Platform> GetAllPlatforms() =>
        _context.Platforms.OrderBy(p => p.Id).ToList();

    public Platform? GetPlatformById(int id) =>
        _context.Platforms.FirstOrDefault(p => p.Id == id);

    public bool SaveChanges() => _context.SaveChanges() >= 0;
}
