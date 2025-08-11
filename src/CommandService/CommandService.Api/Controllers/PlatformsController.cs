using CommandService.Application.AppServices.Interfaces;
using CommandService.Application.Dtos.Platform;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Api.Controllers;

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
