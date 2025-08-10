using CommandService.Domain.Entities;

namespace CommandService.Application.Repositories;

public interface ICommandRepo
{
    bool SaveChanges();

    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command? GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}
