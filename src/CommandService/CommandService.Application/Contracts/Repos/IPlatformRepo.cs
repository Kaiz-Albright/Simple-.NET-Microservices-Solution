using CommandService.Domain.Entities;

namespace CommandService.Application.Contracts.Repos;

public interface IPlatformRepo
{
    bool SaveChanges();

    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExists(int platformId);
}
