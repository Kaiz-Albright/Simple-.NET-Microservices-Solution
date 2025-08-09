using CommandsService.Models;

namespace CommandsService.Data.Repos
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command == null) 
            { 
                throw new ArgumentNullException(nameof(command), "Command cannot be null.");
            }
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null) 
            { 
                throw new ArgumentNullException(nameof(platform), "Platform cannot be null.");
            }

            _context.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms
                .OrderBy(p => p.Id)
                .ToList();
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

        public bool PlatformExists(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
