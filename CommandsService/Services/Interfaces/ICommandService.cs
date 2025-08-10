using CommandsService.Dtos.Command;

namespace CommandsService.Services.Interfaces;

public interface ICommandService
{
    string TestInboundConnection();

    IEnumerable<CommandReadDto> GetCommandsForPlatform(int platformId);
}
