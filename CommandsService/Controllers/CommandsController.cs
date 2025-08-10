using CommandsService.Dtos.Platform;
using CommandsService.Services;
using CommandsService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class CommandsController: ControllerBase
    {
        private readonly ICommandService _commandService;

        public CommandsController(ICommandService commandService)
        {
            _commandService = commandService;
        }

        [HttpPost(Name = "TestInboundConnection")]
        public ActionResult TestInboundConnection()
        {
            var response = _commandService.TestInboundConnection();
            return Ok(response);
        }
    }
}
