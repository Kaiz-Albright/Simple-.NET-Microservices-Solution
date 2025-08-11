using AutoMapper;
using CommandService.Application.AppServices.Interfaces;
using CommandService.Application.Contracts.Repos;
using CommandService.Application.Dtos.Platform;

namespace CommandService.Application.Services;

public class PlatformService : IPlatformService
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;

    public PlatformService(IPlatformRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public IEnumerable<PlatformReadDto> GetAllPlatforms()
    {
        Console.WriteLine("--> Getting Platforms from PlatformService");
        var platforms = _repository.GetAllPlatforms();
        return _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
    }

    public bool PlatformExists(int platformId)
    {
        Console.WriteLine($"--> Checking if platform with ID {platformId} exists");
        return _repository.PlatformExists(platformId);
    }

    public string TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");
        return "Inbound test from Commands Service";
    }
}
