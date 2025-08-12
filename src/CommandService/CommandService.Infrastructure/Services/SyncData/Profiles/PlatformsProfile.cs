using AutoMapper;
using PlatformService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandService.Infrastructure.Services.SyncData.Profiles
{
    public class PlatformsProfile: Profile
    {
        public PlatformsProfile()
        {
            // Source -> Target
            CreateMap<GrpcPlatformModel, Domain.Entities.Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Commands, opt => opt.Ignore());
        }
    }
}
