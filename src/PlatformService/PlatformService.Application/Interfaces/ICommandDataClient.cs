using PlatformService.Application.Dtos;

namespace PlatformService.Infrastructure.Interfaces;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}
