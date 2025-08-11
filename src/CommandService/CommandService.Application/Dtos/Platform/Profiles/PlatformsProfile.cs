using AutoMapper;

namespace CommandService.Application.Dtos.Platform.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Domain.Entities.Platform, PlatformReadDto>();
    }
}
