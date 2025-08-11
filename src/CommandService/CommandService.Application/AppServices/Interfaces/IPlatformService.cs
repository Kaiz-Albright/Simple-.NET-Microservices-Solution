using CommandService.Application.Dtos.Platform;

namespace CommandService.Application.AppServices.Interfaces;

public interface IPlatformService
{
    string TestInboundConnection();


    IEnumerable<PlatformReadDto> GetAllPlatforms();
    bool PlatformExists(int platformId);
}
