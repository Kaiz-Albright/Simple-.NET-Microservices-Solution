using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandService.Application.Dtos
{
    public class GenericEventDto
    {
        public string Event { get; set; }

        public GenericEventDto()
        {
            Event = string.Empty;
        }
    }
}
