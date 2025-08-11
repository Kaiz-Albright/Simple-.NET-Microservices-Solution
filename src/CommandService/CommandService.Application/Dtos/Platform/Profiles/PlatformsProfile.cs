using AutoMapper;

namespace CommandService.Application.Dtos.Platform.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        // Source -> Target
        CreateMap<Domain.Entities.Platform, PlatformReadDto>();
        CreateMap<PlatformPublishedDto, Domain.Entities.Platform>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id));
    }
}
