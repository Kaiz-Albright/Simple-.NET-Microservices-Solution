using AutoMapper;
using PlatformService.Contracts;
using PlatformService.Domain.Entities;

namespace PlatformService.Application.Mappings;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
    }
}
