using AutoMapper;
using PlatformService.Contracts.Dtos;
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
