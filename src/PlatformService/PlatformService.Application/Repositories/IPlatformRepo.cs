using PlatformService.Domain.Entities;

namespace PlatformService.Application.Repositories;

public interface IPlatformRepo
{
    bool SaveChanges();

    IEnumerable<Platform> GetAllPlatforms();
    Platform? GetPlatformById(int id);
    void CreatePlatform(Platform platform);
}
