using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandService.Application.Services.Integration.Interfaces
{
    public interface IEventProcessor
    {
        void ProcessEventAsync(string message);
    }
}
