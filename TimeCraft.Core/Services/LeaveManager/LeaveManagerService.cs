
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using TimeCraft.Core.Dtos;
using TimeCraft.Core.Services.TimeoffBalanceService;
using TimeCraft.Domain.Dtos.LeaveManagerDtos;
using TimeCraft.Domain.Entities;
using TimeCraft.Domain.Enums;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.LeaveManager
{
    public class LeaveManagerService : ILeaveManagerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LeaveManagerService> _logger;
        private readonly ITimeoffBalanceService<TimeoffBalance> _timeoffBalanceService;

        public LeaveManagerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LeaveManagerService> logger, ITimeoffBalanceService<TimeoffBalance> timeoffBalanceService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _timeoffBalanceService = timeoffBalanceService;
        }

        public async Task<bool> ApplyForTimeoffRequest(string userId, TimeoffRequestApplicationDto application)
        {
            var employee = await _unitOfWork.Repository<Employee>().GetAll().Where(x => x.UserId == userId).FirstOrDefaultAsync();
            var user = await _unitOfWork.Repository<User>().GetAll().Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (employee is null)
            {
                return false;
            }

            application.EmployeeId = employee.Id;

            var typeIsCorrect = Enum.TryParse<TimeoffType>(application.Type, out var timeoffType);
            if (!typeIsCorrect)
            {
                throw new Exception("The timeoff type isn't correct");
            }

            var timeoffDays = (application.EndDate - application.StartDate).Days;

            var employeeBalance = _timeoffBalanceService.SearchTimeoffBalances(application.EmployeeId ?? 0).FirstOrDefault();

            if (ExceedsBalance(timeoffType, employeeBalance, timeoffDays))
            {
                throw new Exception("You don't have enough balance!");
            }

            var timeoffRequest = _mapper.Map<TimeoffRequest>(application);

            _unitOfWork.Repository<TimeoffRequest>().Create(timeoffRequest);
            await _unitOfWork.CompleteAsync();

            // Send email to user to notify that the application was succesful
            PublishTimeoffRequestForUser(user.FirstName, user.LastName, user.Email, application.StartDate, application.EndDate);

            // Send email to HR to notify that a new timeoff request was made 
            PublishTimeoffRequestForHR(user.FirstName, user.LastName, user.Email, timeoffRequest.Type, application.StartDate, application.EndDate, timeoffRequest.Comment);

            return true;
        }

        public async Task ApproveTimeoffRequest(int id)
        {
            await ApproveOrDenyTimeoffRequest(id, approve: true);
        }

        public async Task RejectTimeoffRequest(int id)
        {
            await ApproveOrDenyTimeoffRequest(id, deny: true);
        }

        private async Task ApproveOrDenyTimeoffRequest(int id, bool approve = false, bool deny = false)
        {
            var timeoffRequest = await _unitOfWork.Repository<TimeoffRequest>().GetById(x => x.Id == id)
                                                  .Include(x => x.Employee)
                                                  .ThenInclude(x => x.User).FirstOrDefaultAsync();

            if (timeoffRequest is null)
            {
                throw new NullReferenceException("The timeoff request doesn't exist!");
            }

            if (approve && timeoffRequest.Status == TimeoffRequestStatusType.Approved.ToString())
            {
                throw new Exception("The timeoff is already approved!");
            }

            if (deny && timeoffRequest.Status == TimeoffRequestStatusType.Denied.ToString())
            {
                throw new Exception("The timeoff is already denied!");
            }

            string previousStatus = timeoffRequest.Status;

            // If approved decrease the balance for that employee
            if (approve)
            {
                Enum.TryParse<TimeoffType>(timeoffRequest.Type, out var timeoffType);
               
                var timeoffDays = (timeoffRequest.EndDate - timeoffRequest.StartDate).Days;        

                await _timeoffBalanceService.ChangeBalance(timeoffRequest.EmployeeId, -timeoffDays, timeoffType);
            }

            timeoffRequest.Status = approve ? TimeoffRequestStatusType.Approved.ToString() : TimeoffRequestStatusType.Denied.ToString();
            timeoffRequest.UpdatedOn = DateTime.Now;

            _unitOfWork.Repository<TimeoffRequest>().Update(timeoffRequest);
            await _unitOfWork.CompleteAsync();

            var data = new TimeoffRequestStatusDto
            {
                UserFirstName = timeoffRequest.Employee.User.FirstName,
                UserLastName = timeoffRequest.Employee.User.LastName,
                Type = timeoffRequest.Type,
                Comment = timeoffRequest.Comment,
                FromStatus = previousStatus,
                ToStatus = timeoffRequest.Status,
            };

            PublishTimeoffRequestStatus(data); // Send timeoff request status changed email
        }

        /// <summary>
        /// Publishes the event to the "timeoff-request" queue when a new timeoff-request is made
        /// </summary>
        private void PublishTimeoffRequestForUser(string name, string lastName, string email, DateTime startDate, DateTime endDate)
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
        private void PublishTimeoffRequestForHR(string name, string lastName, string email, string type, DateTime startDate, DateTime endDate, string comment)
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


        /// <summary>
        /// Publishes the event to the "timeoff-request" when the timeoff request status is changed
        /// </summary>
        /// <param name="rabbitData"></param>
        private void PublishTimeoffRequestStatus(TimeoffRequestStatusDto data)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "timeoff-request-status",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

                channel.BasicPublish(exchange: "",
                                     routingKey: "timeoff-request-status",
                                     basicProperties: null,
                                     body: body);

                _logger.LogInformation($"{nameof(Employee)} - Data for timeoff-request-status is published to the rabbit!");
            }
        }

        private bool ExceedsBalance(TimeoffType timeoffType, TimeoffBalance employeeBalance, int timeoffDays)
        {
            switch (timeoffType)
            {
                case TimeoffType.Vacation:
                    if (timeoffDays > employeeBalance.VacationDays)
                        return true;
                    break;
                case TimeoffType.Sick:
                    if (timeoffDays > employeeBalance.SickDays)
                        return true;
                    break;
                case TimeoffType.Personal:
                    if (timeoffDays > employeeBalance.PersonalDays)
                        return true;
                    break;
                case TimeoffType.Other:
                    if (timeoffDays > employeeBalance.OtherTimeOffDays)
                        return true;
                    break;
                default:
                    throw new Exception("The given type balance doesn't exist!");
            }
            return false;
        }


    }
}
