using CommandsService.Models;

namespace CommandsService.Data.Repos
{
    public interface ICommandRepo
    {
        bool SaveChanges();

        // Platform related methods
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        bool PlatformExists(int platformId);

        // Command related methods
        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Command? GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}
