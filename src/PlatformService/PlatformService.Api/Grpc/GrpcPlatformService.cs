using AutoMapper;
using Grpc.Core;
using PlatformService.Application.Contracts.Repos;

namespace PlatformService.Api.Grpc
{
    public class GrpcPlatformService: GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepo platformRepo, IMapper mapper)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = _platformRepo.GetAllPlatforms();

            foreach (var platform in platforms)
            {
                var platformDto = _mapper.Map<GrpcPlatformModel>(platform);
                response.Platforms.Add(platformDto);
            }

            return Task.FromResult(response);
        }
    }
}
