using AutoMapper;
using CommandService.Application.Contracts.Services;
using CommandService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using PlatformService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandService.Infrastructure.Services.SyncData
{
    public class PlatformGrpcClient : IPlatformGrpcClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformGrpcClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            var grpcPlatform = _configuration.GetValue<string>("GrpcPlatform");
            if (string.IsNullOrEmpty(grpcPlatform))
            {
                Console.WriteLine("--> Grpc Platform configuration is missing.");
                return Enumerable.Empty<Platform>();
            }

            Console.WriteLine($"--> Calling Grpc Service: {grpcPlatform}");
            try
            {
                using var channel = Grpc.Net.Client.GrpcChannel.ForAddress(grpcPlatform);
                var client = new GrpcPlatform.GrpcPlatformClient(channel);
                var request = new GetAllRequest();

                var response = client.GetAllPlatforms(request);
                Console.WriteLine("--> Grpc Service call successful");
                return _mapper.Map<IEnumerable<Platform>>(response.Platforms);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error calling Grpc Service: {ex.Message}");
                return Enumerable.Empty<Platform>();
            }
        }
    }
}
