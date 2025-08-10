using Microsoft.AspNetCore.Mvc;
using PlatformService.Application.Dtos;
using PlatformService.Application.Services;

namespace PlatformService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformService _platformService;

    public PlatformsController(IPlatformService platformService)
    {
        _platformService = platformService;
    }

    [HttpGet(Name = "GetPlatforms")]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting Platforms from PlatformService");
        var platforms = _platformService.GetAllPlatforms();
        return Ok(platforms);
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        Console.WriteLine($"--> Getting Platform with ID: {id} from PlatformService");
        var platform = _platformService.GetPlatformById(id);
        if (platform == null)
        {
            return NotFound();
        }
        return Ok(platform);
    }

    [HttpPost(Name = "CreatePlatform")]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        Console.WriteLine("--> Creating a new Platform in PlatformService");
        var platformReadDto = await _platformService.CreatePlatformAsync(platformCreateDto);
        if (platformReadDto == null)
        {
            return BadRequest("Invalid platform data.");
        }
        return CreatedAtRoute(
            nameof(GetPlatformById),
            new { Id = platformReadDto.Id },
            platformReadDto
        );
    }
}
