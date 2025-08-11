using AutoMapper;
using CommandService.Application.Dtos.Command;
using CommandService.Application.Repositories;

namespace CommandService.Application.Services;

public class CommandService : ICommandService
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public CommandService(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public IEnumerable<CommandReadDto> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Getting commands for platform with ID {platformId}");

        var commands = _repository.GetCommandsForPlatform(platformId);
        return _mapper.Map<IEnumerable<CommandReadDto>>(commands);
    }

    public IEnumerable<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Getting command with ID {commandId} for platform with ID {platformId}");

        var command = _repository.GetCommand(platformId, commandId);
        return _mapper.Map<IEnumerable<CommandReadDto>>(command);
    }

    public string TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");
        return "Inbound test from Commands Service";
    }
}
