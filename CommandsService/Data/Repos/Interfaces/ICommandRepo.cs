using CommandsService.Models;

namespace CommandsService.Data.Repos.Interfaces
{
    public interface ICommandRepo
    {
        bool SaveChanges();

        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Command? GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}
