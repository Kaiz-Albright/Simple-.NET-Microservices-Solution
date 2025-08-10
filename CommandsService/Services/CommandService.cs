using AutoMapper;
using CommandsService.Data.Repos.Interfaces;
using CommandsService.Dtos.Command;
using CommandsService.Dtos.Platform;
using CommandsService.Services.Interfaces;

namespace CommandsService.Services;

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

    public string TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");
        return "Inbound test from Commands Service";
    }
}
