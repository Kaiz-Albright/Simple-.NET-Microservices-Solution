using Microsoft.AspNetCore.Mvc;
using CommandsService.Services;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ICommandService _commandService;

    public PlatformsController(ICommandService commandService)
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
