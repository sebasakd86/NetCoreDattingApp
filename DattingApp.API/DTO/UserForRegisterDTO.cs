using System.ComponentModel.DataAnnotations;

namespace DattingApp.API.DTO
{
    public class UserForRegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(10, MinimumLength=6, ErrorMessage="You must specify password between 6 and 10 characters")]
        public string Password { get; set; }
    }
}