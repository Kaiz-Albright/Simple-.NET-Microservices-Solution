using CommandService.Application.Dtos.Platform;

namespace CommandService.Application.Services;

public interface IPlatformService
{
    IEnumerable<PlatformReadDto> GetAllPlatforms();
    bool PlatformExists(int platformId);
}
