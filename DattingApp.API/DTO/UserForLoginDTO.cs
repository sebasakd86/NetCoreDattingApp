using System.ComponentModel.DataAnnotations;

namespace DattingApp.API.DTO
{
    public class UserForLoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}