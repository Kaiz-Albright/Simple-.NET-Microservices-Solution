using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Application.Dtos;

public class PlatformPublishedDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Event { get; set; }


    public PlatformPublishedDto()
    {
        Name = string.Empty;
        Event = string.Empty;
    }
}



