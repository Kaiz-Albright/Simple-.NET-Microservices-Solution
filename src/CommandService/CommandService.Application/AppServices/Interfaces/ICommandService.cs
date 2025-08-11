using CommandService.Application.Dtos.Command;

namespace CommandService.Application.AppServices.Interfaces;

public interface ICommandService
{
    string TestInboundConnection();

    IEnumerable<CommandReadDto> GetCommandsForPlatform(int platformId);
    IEnumerable<CommandReadDto> GetCommandForPlatform(int platformId, int commandId);
    CommandReadDto CreateCommand(int platformId, CommandCreateDto commandCreateDto);
}
