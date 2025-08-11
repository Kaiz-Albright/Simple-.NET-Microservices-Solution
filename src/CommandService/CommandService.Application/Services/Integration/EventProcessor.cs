using AutoMapper;
using CommandService.Application.Contracts.Repos;
using CommandService.Application.Dtos;
using CommandService.Application.Dtos.Platform;
using CommandService.Application.Services.Integration.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommandService.Application.Services.Integration
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEventAsync(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                case EventType.Undetermined:
                    Console.WriteLine($"--> Could not process Undetermined event: {message}");
                    break;
                default:
                    Console.WriteLine($"--> Unknown event type: {eventType}");
                    break;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var platformRepo = scope.ServiceProvider.GetRequiredService<IPlatformRepo>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                if (platformPublishedDto == null)
                {
                    Console.WriteLine($"--> Could not deserialize PlatformPublishedDto from message: {platformPublishedMessage}");
                    return;
                }
                try
                {
                    var platform = _mapper.Map<Domain.Entities.Platform>(platformPublishedDto);
                    if (!platformRepo.ExternalPlatformExists(platform.ExternalID))
                    {
                        platformRepo.CreatePlatform(platform);
                        platformRepo.SaveChanges();
                        Console.WriteLine($"--> Platform added: {platform.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"--> Platform already exists: {platform.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Error adding platform: {ex.Message}");
                }
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine($"--> Determining Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            if (eventType == null || string.IsNullOrEmpty(eventType.Event))
            {
                Console.WriteLine($"--> Could not determine event type from message: {notificationMessage}");
                return EventType.Undetermined;
            }

            switch(eventType.Event.ToLower())
            {
                case "platform_published":
                    Console.WriteLine($"--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine($"--> Could not determine event type from message: {notificationMessage}");
                    return EventType.Undetermined;
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
