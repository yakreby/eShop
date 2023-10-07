using eShop.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace eShop.Services.EmailAPI.Messaging
{
    public class RabbitMQAuthConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public RabbitMQAuthConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            ;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, false, false, null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                string email = JsonConvert.DeserializeObject<string>(content);
                HandleMessage(email);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"), false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(string email)
        {
            _emailService.RegisterUserEmailAndLog(email).GetAwaiter().GetResult();
        }
    }
}
