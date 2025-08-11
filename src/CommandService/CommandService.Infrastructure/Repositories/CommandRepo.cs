using CommandService.Application.Contracts.Repos;
using CommandService.Domain.Entities;
using CommandService.Infrastructure.Data;

namespace CommandService.Infrastructure.Repositories;

public class CommandRepo : ICommandRepo
{
    private readonly CommandDbContext _context;

    public CommandRepo(CommandDbContext context)
    {
        _context = context;
    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command), "Command cannot be null.");
        }

        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }

    public Command? GetCommand(int platformId, int commandId)
    {
        return _context.Commands
            .Where(c => c.PlatformId == platformId && c.Id == commandId)
            .FirstOrDefault();
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Id)
            .ToList();
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
