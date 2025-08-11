using AutoMapper;

namespace CommandService.Application.Dtos.Command.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        // Source -> Target
        CreateMap<CommandCreateDto, Domain.Entities.Command>();
        CreateMap<Domain.Entities.Command, CommandReadDto>();
    }
}
