using PlatformService.Application.Dtos;

namespace PlatformService.Application.AppServices.Interfaces;

public interface IPlatformService
{
    IEnumerable<PlatformReadDto> GetAllPlatforms();
    PlatformReadDto? GetPlatformById(int id);
    Task<PlatformReadDto?> CreatePlatformAsync(PlatformCreateDto platformCreateDto);
}
