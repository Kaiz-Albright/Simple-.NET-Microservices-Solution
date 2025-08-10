using PlatformService.Contracts;

namespace PlatformService.Application.Interfaces;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}
