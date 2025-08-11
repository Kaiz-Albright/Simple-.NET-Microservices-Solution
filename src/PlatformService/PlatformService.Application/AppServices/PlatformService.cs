using AutoMapper;
using PlatformService.Domain.Entities;
using PlatformService.Application.Dtos;
using PlatformService.Application.Contracts.Services;
using PlatformService.Application.Contracts.Repos;
using PlatformService.Application.AppServices.Interfaces;

namespace PlatformService.Application.Services;

public class PlatformService : IPlatformService
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformService(
        IPlatformRepo repository,
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
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

        // Send Sync Message
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error sending platform to Command Service: {ex.Message}");
        }

        //Send Async Message
        try
        {
            var platformPuishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPuishedDto.Event = "Platform_Published";
            await _messageBusClient.PublishNewPlatform(platformPuishedDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error publishing new platform to message bus: {ex.Message}");
        }

        return platformReadDto;
    }
}
