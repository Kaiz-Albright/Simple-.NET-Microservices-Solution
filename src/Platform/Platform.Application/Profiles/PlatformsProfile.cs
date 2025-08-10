using AutoMapper;
using Company.Platform.Domain.Models;
using Company.Platform.Application.Dtos;

namespace Company.Platform.Application.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        // Source -> Target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
    }
}
