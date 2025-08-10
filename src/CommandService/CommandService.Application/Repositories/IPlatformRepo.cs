using CommandService.Domain.Entities;

namespace CommandService.Application.Repositories;

public interface IPlatformRepo
{
    bool SaveChanges();

    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExists(int platformId);
}
