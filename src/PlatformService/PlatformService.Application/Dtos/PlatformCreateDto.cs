using System.ComponentModel.DataAnnotations;

namespace PlatformService.Application.Dtos;

public class PlatformCreateDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Publisher { get; set; }

    [Required]
    public string Cost { get; set; }

    public PlatformCreateDto()
    {
        Name = string.Empty;
        Publisher = string.Empty;
        Cost = string.Empty;
    }

    public PlatformCreateDto(string name, string publisher, string cost)
    {
        Name = name;
        Publisher = publisher;
        Cost = cost;
    }
}
