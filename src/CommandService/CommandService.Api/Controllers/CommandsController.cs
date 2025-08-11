using CommandService.Application.AppServices.Interfaces;
using CommandService.Application.Dtos.Command;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Api.Controllers;

[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandService _commandService;
    private readonly IPlatformService _platformService;

    public CommandsController(ICommandService commandService, IPlatformService platformService)
    {
        _commandService = commandService;
        _platformService = platformService;
    }

    [HttpGet(Name = "GetCommandsForPlatform")]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        try
        {
            Console.WriteLine($"--> Getting commands for platform with ID {platformId}");
            if (platformId <= 0)
            {
                return BadRequest("Platform ID must be greater than zero.");
            }
            if (!_platformService.PlatformExists(platformId))
            {
                return NotFound($"Platform with ID {platformId} does not exist.");
            }

            var commands = _commandService.GetCommandsForPlatform(platformId);
            if (commands == null || !commands.Any())
            {
                return NotFound($"No commands found for platform with ID {platformId}.");
            }
            return Ok(commands);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error in {nameof(GetCommandsForPlatform)}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        try
        {
            Console.WriteLine($"--> Getting command with ID {commandId} for platform with ID {platformId}");
            if (platformId <= 0 || commandId <= 0)
            {
                return BadRequest("Platform ID / Command ID must be greater than zero.");
            }

            if (!_platformService.PlatformExists(platformId))
            {
                return NotFound($"Platform with ID {platformId} does not exist.");
            }
            
            var command = _commandService.GetCommandForPlatform(platformId, commandId);
            if (command == null || !command.Any())
            {
                return NotFound($"No command found with ID {commandId} for platform with ID {platformId}.");
            }
            return Ok(command);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error in {nameof(GetCommandForPlatform)}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost(Name = "CreateCommandForPlatform")]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
    {
        try
        {
            Console.WriteLine($"--> Creating command for platform with ID {platformId}");
            if (platformId <= 0)
            {
                return BadRequest("Platform ID must be greater than zero.");
            }
            if (!_platformService.PlatformExists(platformId))
            {
                return NotFound($"Platform with ID {platformId} does not exist.");
            }
            if (commandCreateDto == null)
            {
                return BadRequest("Command data cannot be null.");
            }
            var commandReadDto = _commandService.CreateCommand(platformId, commandCreateDto);
            return CreatedAtRoute(
                nameof(GetCommandForPlatform),
                new { platformId = platformId, commandId = commandReadDto.Id },
                commandReadDto
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error in {nameof(CreateCommandForPlatform)}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    /*
    [HttpPost(Name = "TestInboundConnection")]
    public ActionResult TestInboundConnection()
    {
        var response = _commandService.TestInboundConnection();
        return Ok(response);
    }
    */
}
