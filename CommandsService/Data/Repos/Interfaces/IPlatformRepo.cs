using CommandsService.Models;

namespace CommandsService.Data.Repos.Interfaces
{
    public interface IPlatformRepo
    {
        bool SaveChanges();


        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        bool PlatformExists(int platformId);
    }
}
