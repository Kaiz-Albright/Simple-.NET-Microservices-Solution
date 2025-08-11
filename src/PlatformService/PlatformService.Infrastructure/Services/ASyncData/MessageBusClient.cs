using Microsoft.Extensions.Configuration;
using PlatformService.Application.Contracts.Services;
using PlatformService.Application.Dtos;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Infrastructure.Services.ASyncData
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _congifuration;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _congifuration = configuration;
            _connection = ConnectToMessageBus();
            _channel = CreateMessageChannel();

            SetEvents();
        }

        private void SetEvents()
        {
            _connection.ConnectionShutdownAsync += (sender, e) =>
            {
                Console.WriteLine("--> Connection to Message Bus has been shutdown");
                return Task.CompletedTask;
            };
            _connection.CallbackExceptionAsync += (sender, e) =>
            {
                Console.WriteLine($"--> Callback Exception: {e.Exception.Message}");
                return Task.CompletedTask;
            };
            _connection.ConnectionBlockedAsync += (sender, e) =>
            {
                Console.WriteLine("--> Connection to Message Bus is blocked");
                return Task.CompletedTask;
            };
        }

        private IChannel CreateMessageChannel()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                throw new InvalidOperationException("Connection to Message Bus is not established.");
            }

            try
            {
                Console.WriteLine("--> Creating Message Channel");
                IChannel channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
                Console.WriteLine("--> Message Channel created");

                // Declare an exchange for the trigger
                channel.ExchangeDeclareAsync(
                    exchange: "trigger",
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false,
                    arguments: null
                ).GetAwaiter().GetResult();

                return channel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error creating Message Channel: {ex.Message}");
                throw;
            }
        }

        private IConnection ConnectToMessageBus()
        {
            var factory = ConfigureMessageBusConnection();

            try
            {
                Console.WriteLine("--> Attempting to connect to Message Bus");
                IConnection connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                Console.WriteLine("--> Connected to Message Bus");
                return connection;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not connect to Message Bus: {ex.Message}");
                throw;
            }
        }
        private ConnectionFactory ConfigureMessageBusConnection()
        {
            string? hostName = _congifuration.GetValue<string>("RabbitMQHost");
            int? port = _congifuration.GetValue<int>("RabbitMQPort");

            if (string.IsNullOrEmpty(hostName) || !port.HasValue)
            {
                throw new ArgumentException("RabbitMQ host or port is not configured properly.");
            }

            Console.WriteLine($"--> RabbitMQ Host: {hostName}, Port: {port}");
            return new ConnectionFactory()
            {
                HostName = hostName,
                Port = (int)port
            };
        }

        public async Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = System.Text.Json.JsonSerializer.Serialize(platformPublishedDto);

            if (_channel == null || !_channel.IsOpen)
            {
                throw new InvalidOperationException("Message Channel is not open.");
            }

            try
            {
                await SendMessage(message);
                Console.WriteLine($"--> Published message to Message Bus: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error publishing message to Message Bus: {ex.Message}");
            }
        }

        private async Task SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            var props = new BasicProperties();

            await _channel.BasicPublishAsync(
                exchange: "trigger",
                routingKey: "",
                mandatory: false,
                basicProperties: props,
                body: body
            );
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_channel != null && _channel.IsOpen)
                {
                    Console.WriteLine("--> Closing Message Channel");
                    _channel.CloseAsync().GetAwaiter().GetResult();
                }
                if (_connection != null && _connection.IsOpen)
                {
                    Console.WriteLine("--> Closing Message Bus Connection");
                    _connection.CloseAsync().GetAwaiter().GetResult();
                }
            }
        }
    }
}
