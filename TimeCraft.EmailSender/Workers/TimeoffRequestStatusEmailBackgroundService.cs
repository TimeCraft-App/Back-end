using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeCraft.EmailSender.Dtos;

namespace TimeCraft.EmailSender.Workers
{
    public class TimeoffStatusRequestEmailBackgroundService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<TimeoffStatusRequestEmailBackgroundService> _logger;

        public TimeoffStatusRequestEmailBackgroundService(IEmailSender emailSender, ILogger<TimeoffStatusRequestEmailBackgroundService> logger)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _emailSender = emailSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel.QueueDeclare("timeoff-request-status", exclusive: false, durable: true, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, args) =>
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var data = JsonConvert.DeserializeObject<TimeoffRequestStatusDto>(message);

                SendEmail(data).GetAwaiter().GetResult();
            };

            _channel.BasicConsume(queue: "timeoff-request-status",
                                  autoAck: true,
                                  consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(30000, stoppingToken);
            }
        }

        private async Task SendEmail(TimeoffRequestStatusDto data)
        {
            var pathToFile = "Templates/timeoffRequestStatus.html";

            string htmlBody = "";
            using (StreamReader streamReader = File.OpenText(pathToFile))
            {
                htmlBody = streamReader.ReadToEnd();
            }

            var contentData = new string[] {
                data.UserFirstName + " " + data.UserLastName,
                data.Type,
                data.FromStatus,
                data.ToStatus,
                data.Comment ?? "",
                DateTime.Now.ToString("dd/MM/yyyy")
            };

            var content = string.Format(htmlBody, contentData);

            try
            {
                _logger.LogInformation("Sending 'Timeoff request status' email!");

                // Todo: send to HR department
                await _emailSender.SendEmailAsync("jetonsllamniku@gmail.com", "Timeoff request status changed", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email for timeoff request!");
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
