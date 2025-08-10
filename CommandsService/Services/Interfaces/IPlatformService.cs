namespace CommandsService.Services.Interfaces
{
    public interface IPlatformService
    {
        IEnumerable<Dtos.Platform.PlatformReadDto> GetAllPlatforms();
    }
}
