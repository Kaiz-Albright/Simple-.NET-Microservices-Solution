using CommandsService.Dtos.Command;
using CommandsService.Dtos.Platform;
using CommandsService.Services;
using CommandsService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController: ControllerBase
    {
        private readonly ICommandService _commandService;

        public CommandsController(ICommandService commandService)
        {
            _commandService = commandService;
        }

        [HttpGet(Name = "GetCommandsForPlatform")]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            try
            {
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

        [HttpPost(Name = "TestInboundConnection")]
        public ActionResult TestInboundConnection()
        {
            var response = _commandService.TestInboundConnection();
            return Ok(response);
        }
    }
}
