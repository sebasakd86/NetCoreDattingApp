using System;
using System.Collections.Generic;
using DattingApp.API.Model;

namespace DattingApp.API.DTO
{
    public class UserForListDTO
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        //public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public string KnownAs {get;set;}
        public string Interests {get;set;}
    }
}