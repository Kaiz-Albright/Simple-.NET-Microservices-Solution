using AutoMapper;
using CommandsService.Data.Repos.Interfaces;
using CommandsService.Dtos.Platform;
using CommandsService.Services.Interfaces;

namespace CommandsService.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        public PlatformService(IPlatformRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<PlatformReadDto> GetAllPlatforms()
        {
            Console.WriteLine("--> Getting Platforms from PlatformsService");
            var platforms = _repository.GetAllPlatforms();
            return _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
        }
    }
}
