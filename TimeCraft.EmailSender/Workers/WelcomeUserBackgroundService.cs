using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeCraft.EmailSender.Dtos;

namespace TimeCraft.EmailSender.Workers
{
    public class WelcomeUserBackgroundService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<WelcomeUserBackgroundService> _logger;

        public WelcomeUserBackgroundService(IEmailSender emailSender, ILogger<WelcomeUserBackgroundService> logger)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _emailSender = emailSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel.QueueDeclare("welcome-user", exclusive: false, durable: true, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, args) =>
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var data = JsonConvert.DeserializeObject<WelcomeUserDto>(message);

                SendEmail(data).GetAwaiter().GetResult();
            };

            _channel.BasicConsume(queue: "welcome-user",
                                  autoAck: true,
                                  consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(30000, stoppingToken);
            }
        }

        private async Task SendEmail(WelcomeUserDto data)
        {
            var content = data.Info;

            try
            {
                _logger.LogInformation("Sending 'Welcome to TimeCraft' email!");
                
                await _emailSender.SendEmailAsync(data.Email, "Welcome to TimeCraft!", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email for welcome to timecraft!");
            }
        }
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
