using System.ComponentModel.DataAnnotations;

namespace TimeCraft.Domain.Dtos.UserDtos
{
    public class UserLogin
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
