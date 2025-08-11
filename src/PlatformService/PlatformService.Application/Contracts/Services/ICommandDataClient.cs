using PlatformService.Application.Dtos;

namespace PlatformService.Application.Contracts.Services;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}
