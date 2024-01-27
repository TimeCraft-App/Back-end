using AutoMapper;
using Microsoft.Extensions.Logging;
using TimeCraft.Domain.Entities;
using TimeCraft.Infrastructure.Persistence.UnitOfWork;

namespace TimeCraft.Core.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
    }
}
