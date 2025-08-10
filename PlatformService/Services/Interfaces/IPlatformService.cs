using PlatformService.Dtos;

namespace PlatformService.Services.Interfaces;

public interface IPlatformService
{
    IEnumerable<PlatformReadDto> GetAllPlatforms();
    PlatformReadDto? GetPlatformById(int id);
    Task<PlatformReadDto?> CreatePlatformAsync(PlatformCreateDto platformCreateDto);
}
