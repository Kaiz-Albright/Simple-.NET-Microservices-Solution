using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandService.Application.Contracts.Services
{
    public interface IMessageBusSubscriber
    {
        Task SubscribeAsync();
        IChannel GetChannel();
        string GetQueueName();
    }
}
