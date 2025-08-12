using CommandService.Application.Contracts.Repos;
using CommandService.Application.Contracts.Services;
using CommandService.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandService.Infrastructure.Data
{
    public class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformGrpcClient>();

                var platforms = grpcClient.ReturnAllPlatforms();

                SeedData(serviceScope.ServiceProvider.GetRequiredService<IPlatformRepo>(), platforms);  
            }
        }

        private static void SeedData(IPlatformRepo repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms...");

            foreach (var platform in platforms)
            {
                if (!repo.ExternalPlatformExists(platform.ExternalID))
                {
                    repo.CreatePlatform(platform);
                    
                    Console.WriteLine($"--> Platform with ID {platform.ExternalID} created.");
                }
                else
                {
                    Console.WriteLine($"--> Platform with ID {platform.ExternalID} already exists. Skipping creation.");
                }
            }

            repo.SaveChanges();
        }
    }
}
