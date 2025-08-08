namespace CommandsService.Services;

public class CommandService : ICommandService
{
    public string TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");
        return "Inbound test from Commands Service";
    }
}
