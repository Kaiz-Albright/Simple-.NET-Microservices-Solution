using CommandsService.Dtos.Platform;
using CommandsService.Services;
using CommandsService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformService _platformService;

    public PlatformsController(IPlatformService platformService)
    {
        _platformService = platformService;
    }

    [HttpGet(Name = "GetPlatforms")]
    public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
    {
        try
        {
            var platforms = _platformService.GetAllPlatforms();
            return Ok(platforms);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Error in {nameof(GetAllPlatforms)}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
