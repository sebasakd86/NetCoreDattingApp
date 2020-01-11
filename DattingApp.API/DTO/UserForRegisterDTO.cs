using System;
using System.ComponentModel.DataAnnotations;

namespace DattingApp.API.DTO
{
    public class UserForRegisterDTO
    {
        public UserForRegisterDTO()
        {
            this.LastActive = this.Created = DateTime.Now;
        }

        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "You must specify password between 6 and 10 characters")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}