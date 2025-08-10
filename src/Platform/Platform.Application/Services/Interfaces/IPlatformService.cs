using Company.Platform.Application.Dtos;

namespace Company.Platform.Application.Services.Interfaces;

public interface IPlatformService
{
    IEnumerable<PlatformReadDto> GetAllPlatforms();
    PlatformReadDto? GetPlatformById(int id);
    Task<PlatformReadDto?> CreatePlatformAsync(PlatformCreateDto platformCreateDto);
}
