using PlatformService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Application.Contracts.Services
{
    public interface IMessageBusClient
    {
        Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
    }
}
