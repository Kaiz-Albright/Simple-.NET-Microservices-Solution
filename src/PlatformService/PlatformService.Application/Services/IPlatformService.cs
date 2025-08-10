using PlatformService.Contracts.Dtos;

namespace PlatformService.Application.Services;

public interface IPlatformService
{
    IEnumerable<PlatformReadDto> GetAllPlatforms();
    PlatformReadDto? GetPlatformById(int id);
    Task<PlatformReadDto?> CreatePlatformAsync(PlatformCreateDto platformCreateDto);
}
