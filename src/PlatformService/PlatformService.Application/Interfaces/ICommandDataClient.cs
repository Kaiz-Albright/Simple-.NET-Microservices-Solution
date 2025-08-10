using PlatformService.Contracts.Dtos;

namespace PlatformService.Application.Interfaces;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}
