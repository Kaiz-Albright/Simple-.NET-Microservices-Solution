using AutoMapper;
using PlatformService.Application.Dtos;
using PlatformService.Domain.Entities;

namespace PlatformService.Application.Dtos.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        // Source -> Target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
        CreateMap<PlatformReadDto, PlatformPublishedDto>(); 
    }
}
