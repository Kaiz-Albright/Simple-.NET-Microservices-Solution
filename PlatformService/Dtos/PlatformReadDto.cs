using System.ComponentModel.DataAnnotations;

namespace PlatformService.Dtos
{
    public class PlatformReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Publisher { get; set; }

        public string Cost { get; set; }


        public PlatformReadDto()
        {
            Name = string.Empty;
            Publisher = string.Empty;
            Cost = string.Empty;
        }

        public PlatformReadDto(int id, string name, string publisher, string cost)
        {
            Id = id;
            Name = name;
            Publisher = publisher;
            Cost = cost;
        }
    }
}
