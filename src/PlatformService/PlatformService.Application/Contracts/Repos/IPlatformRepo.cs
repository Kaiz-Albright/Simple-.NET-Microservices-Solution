using PlatformService.Domain.Entities;

namespace PlatformService.Application.Contracts.Repos;

public interface IPlatformRepo
{
    bool SaveChanges();

    IEnumerable<Platform> GetAllPlatforms();
    Platform? GetPlatformById(int id);
    void CreatePlatform(Platform platform);
}
