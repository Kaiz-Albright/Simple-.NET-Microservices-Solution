using Company.Platform.Application.Dtos;

namespace Company.Platform.Application.SyncDataServices.Http;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}
