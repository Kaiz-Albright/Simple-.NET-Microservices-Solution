using AutoMapper;
using CommandService.Application.Dtos.Command;
using CommandService.Domain.Entities;

namespace CommandService.Application.Mappings;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<CommandCreateDto, Command>();
        CreateMap<Command, CommandReadDto>();
    }
}
