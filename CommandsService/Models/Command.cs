using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models
{
    public class Command
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string HowTo { get; set; } = string.Empty;

        [Required]
        public string CommandLine { get; set; } = string.Empty;

        [Required]
        public int PlatformId { get; set; }

        public Platform Platform { get; set; } = null!;

        public Command(Platform platform, string howTo, string commandLine)
        {
            Platform = platform;
            PlatformId = platform.Id;
            HowTo = howTo;
            CommandLine = commandLine;
        }

        public Command()
        {

        }
    }
}
