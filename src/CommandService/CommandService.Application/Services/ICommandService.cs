using CommandService.Application.Dtos.Command;

namespace CommandService.Application.Services;

public interface ICommandService
{
    string TestInboundConnection();

    IEnumerable<CommandReadDto> GetCommandsForPlatform(int platformId);
}
