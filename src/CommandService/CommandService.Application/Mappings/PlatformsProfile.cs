using AutoMapper;
using CommandService.Application.Dtos.Platform;
using CommandService.Domain.Entities;

namespace CommandService.Application.Mappings;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
    }
}
