using AutoMapper;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Services;

public class PlatformService : IPlatformService
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;

    public PlatformService(
        IPlatformRepo repository,
        IMapper mapper,
        ICommandDataClient commandDataClient)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
    }

    public IEnumerable<PlatformReadDto> GetAllPlatforms()
    {
        var platforms = _repository.GetAllPlatforms();
        return _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
    }

    public PlatformReadDto? GetPlatformById(int id)
    {
        var platform = _repository.GetPlatformById(id);
        return platform == null ? null : _mapper.Map<PlatformReadDto>(platform);
    }

    public async Task<PlatformReadDto?> CreatePlatformAsync(PlatformCreateDto platformCreateDto)
    {
        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        _repository.CreatePlatform(platformModel);
        _repository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error sending platform to Command Service: {ex.Message}");
        }

        return platformReadDto;
    }
}
