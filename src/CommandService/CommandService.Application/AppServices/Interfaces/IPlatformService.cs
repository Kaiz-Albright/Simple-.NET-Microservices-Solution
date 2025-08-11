using CommandService.Application.Dtos.Platform;

namespace CommandService.Application.AppServices.Interfaces;

public interface IPlatformService
{
    IEnumerable<PlatformReadDto> GetAllPlatforms();
    bool PlatformExists(int platformId);
}
