using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeCraft.Core.Helpers;
using TimeCraft.Domain.Dtos.UserDtos;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private JWTConfiguration _jwtConfiguration;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<JWTConfiguration> jwtConfiguration, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtConfiguration = jwtConfiguration.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<List<User>> GetUsers(int page = 1, int pageSize = 10)
        {
            var users = await _unitOfWork.Repository<User>().GetAll().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return users;
        }
        public async Task<User> GetUser(string id)
        {
            var user = await _unitOfWork.Repository<User>().GetById(x => x.Id == id).FirstOrDefaultAsync();
            return user;
        }
        public async Task<bool> CreateUser(CreateUserDto userToCreate)
        {
            var user = _mapper.Map<User>(userToCreate);
            user.Id = Guid.NewGuid().ToString();
            _unitOfWork.Repository<User>().Create(user);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task UpdateUser(UpdateUserDto userDto)
        {
            var existingUser = await GetUser(userDto.Id);

            if (existingUser == null)
            {
                throw new Exception("The user with the given id doesn't exits!");
            }
            existingUser.Address = userDto.Address;
            existingUser.Email = userDto.Email;
            existingUser.UserName = userDto.UserName;
            existingUser.Role = userDto.Role;
            _unitOfWork.Repository<User>().Update(existingUser);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteUser(string id)
        {
            var existingUser = await GetUser(id);
            if (existingUser == null)
            {
                throw new NullReferenceException("No user with this id exists!");
            }
            _unitOfWork.Repository<User>().Delete(existingUser);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<AuthResult> RegisterUser(UserRegistration user)
        {
            var existingUserEmail = await _userManager.FindByEmailAsync(user.Email);
            var existingUserName = await _userManager.FindByNameAsync(user.UserName);

            if (existingUserEmail != null)
            {
                return new AuthResult
                {
                    Errors = new List<string> { "User with that email already exists" },
                    Succedded = false
                };
            }
            else if (existingUserName != null)
            {
                return new AuthResult
                {
                    Errors = new List<string> { "User with that username already exists" },
                    Succedded = false
                };
            }
            var newUser = new IdentityUser()
            {
                Email = user.Email,
                UserName = user.UserName,

            };
            var created = await _userManager.CreateAsync(newUser, user.Password);

            if (created.Succeeded)
            {
                var userToBeAdded = new User
                {
                    Id = newUser.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Birthday = user.Birthday,
                    Email = user.Email,
                    Address = user.Address,
                    UserName = user.UserName,
                    Role = "User",
                };

                _unitOfWork.Repository<User>().Create(userToBeAdded);
                await _unitOfWork.CompleteAsync();
                var jwtToken = await GenerateTheJWT(newUser);

                PublishRegistrationEvent(userToBeAdded.Email, $"Hey, {userToBeAdded.FirstName}! Welcome To TimeCraft management application. Hope you enjoy it!");

                return new AuthResult
                {
                    Token = jwtToken,
                    Succedded = true,
                };
            }
            else
            {
                return new AuthResult
                {
                    Errors = created.Errors.Select(error => error.Description).ToList(),
                    Succedded = false
                };
            }
        }

        /// <summary>
        /// Publishes the registration event to the "welcome-user" queue 
        /// </summary>
        /// <param name="rabbitData"></param>
        public void PublishRegistrationEvent(string email,  string info)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "welcome-user",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { email, info }));

                channel.BasicPublish(exchange: "",
                                     routingKey: "welcome-user",
                                     basicProperties: null,
                                     body: body);

                _logger.LogInformation($"{nameof(UserService)} - Data for welcome is published to the rabbit!");
            }
        }

        private async Task<string> GenerateTheJWT(IdentityUser user)
        {
            var userInOurDb = await GetUser(user.Id);
            var role = userInOurDb != null ? userInOurDb.Role : "User";

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, role),
                }),
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = "http://localhost:37997",
                Audience = "http://localhost:37997"
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
        public async Task<AuthResult> Login(UserLogin user)
        {
            var existingUser = await _userManager.FindByNameAsync(user.UserName);

            if (existingUser == null) // Check if user exists
            {
                return new AuthResult
                {
                    Errors = new List<string> { "This user does not exist!" },
                    Succedded = false
                };
            }
            var validUser = await _userManager.CheckPasswordAsync(existingUser, user.Password);
            if (!validUser)
            {
                return new AuthResult()
                {
                    Errors = new List<string> { "Incorrect Password!" },
                    Succedded = false
                };
            }

            var jwtToken = await GenerateTheJWT(existingUser);

            return new AuthResult()
            {
                Token = jwtToken,
                Succedded = true
            };
        }
    }

}
