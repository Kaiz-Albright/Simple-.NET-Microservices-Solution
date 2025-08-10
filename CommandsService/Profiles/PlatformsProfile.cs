using AutoMapper;
using CommandsService.Dtos.Platform;
using CommandsService.Models;

namespace CommandsService.Profiles
{
    public class PlatformsProfile: Profile
    {
        public PlatformsProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
        }
    }
}
