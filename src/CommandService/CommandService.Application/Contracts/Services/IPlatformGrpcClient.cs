using CommandService.Application.Dtos.Platform;
using CommandService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandService.Application.Contracts.Services
{
    public interface IPlatformGrpcClient
    {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}
