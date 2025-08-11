
using CommandService.Application.Contracts.Services;
using CommandService.Application.Services.Integration.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.Api.Consumers
{
    public class PlatformCreatedBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private readonly IMessageBusSubscriber _messageBusSubscriber;

        public PlatformCreatedBackgroundService(
            IConfiguration configuration,
            IEventProcessor eventProcessor,
            IMessageBusSubscriber messageBusSubscriber)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            _messageBusSubscriber = messageBusSubscriber;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            // Initialize the event processor with necessary configurations
            _messageBusSubscriber.SubscribeAsync(); 
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var channel = _messageBusSubscriber.GetChannel();
            var queueName = _messageBusSubscriber.GetQueueName();

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    Console.WriteLine("--> Event received from RabbitMQ");

                    var body = ea.Body;
                    var message = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                    _eventProcessor.ProcessEventAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Error processing message: {ex.Message}");
                }
            };

            channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer
            );

            return Task.CompletedTask;  
        }
    }
}
