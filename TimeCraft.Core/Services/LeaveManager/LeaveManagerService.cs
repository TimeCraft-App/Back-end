
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using TimeCraft.Core.Dtos;
using TimeCraft.Domain.Dtos.LeaveManagerDtos;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.LeaveManager
{
    public class LeaveManagerService : ILeaveManagerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LeaveManagerService> _logger;

        public LeaveManagerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LeaveManagerService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> ApplyForTimeoffRequest(string userId, TimeoffRequestApplicationDto application)
        {
            var employee = await _unitOfWork.Repository<Employee>().GetAll().Where(x => x.UserId == userId).FirstOrDefaultAsync();
            var user = await _unitOfWork.Repository<User>().GetAll().Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (employee is null)
            {
                return false;
            }

            application.EmployeeId = employee?.Id;

            var timeoffRequest = _mapper.Map<TimeoffRequest>(application);

            _unitOfWork.Repository<TimeoffRequest>().Create(timeoffRequest);
            await _unitOfWork.CompleteAsync();

            PublishTimeoffRequestForUser(user.FirstName, user.LastName, user.Email, application.StartDate, application.EndDate);
            PublishTimeoffRequestForHR(user.FirstName, user.LastName, user.Email, timeoffRequest.Type, application.StartDate, application.EndDate, timeoffRequest.Comment);

            return true;
        }

        public async Task ApproveTimeoffRequest(int id)
        {
            var timeoffRequest = await _unitOfWork.Repository<TimeoffRequest>().GetById(x => x.Id == id).FirstOrDefaultAsync();

            if (timeoffRequest is null)
            {
                throw new NullReferenceException("The timeoff request doesn't exist!");
            }

            timeoffRequest.Status = "Approved";
            timeoffRequest.UpdatedOn = DateTime.Now;



            _unitOfWork.Repository<TimeoffRequest>().Update(timeoffRequest);
            await _unitOfWork.CompleteAsync();
        }

        public Task RejectTimeoffRequest(int id)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Publishes the event to the "timeoff-request" queue when a new timeoff-request is made
        /// </summary>
        public void PublishTimeoffRequestForUser(string name, string lastName, string email, DateTime startDate, DateTime endDate)
        {
            var info = $"Hello, {name} {lastName}! \n" +
                $" The application for timeoffRequest for dates {startDate.ToString("dd/MM/yyyy")}-{endDate.ToString("dd/MM/yyyy")} \n" +
                $" has been <b>submitted successfully</b>! ";
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "timeoff-request-user",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { email, info }));

                channel.BasicPublish(exchange: "",
                                     routingKey: "timeoff-request-user",
                                     basicProperties: null,
                                     body: body);

                _logger.LogInformation($"{nameof(Employee)} - Data for timeoff-request for user is published to the rabbit!");
            }
        }

        /// <summary>
        /// Publishes the event to the "timeoff-request" queue when a new timeoff-request is made
        /// </summary>
        public void PublishTimeoffRequestForHR(string name, string lastName, string email, string type, DateTime startDate, DateTime endDate, string comment)
        {
            var data = new TimeoffRequestHRDto
            {
                UserFirstName = name,
                UserLastName = lastName,
                Type = type,
                StartDate = startDate,
                EndDate = endDate,
                Comment = comment
            };

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "timeoff-hr",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

                channel.BasicPublish(exchange: "",
                                     routingKey: "timeoff-hr",
                                     basicProperties: null,
                                     body: body);

                _logger.LogInformation($"{nameof(Employee)} - Data for timeoff-request for HR is published to the rabbit!");
            }
        }

    }
}
